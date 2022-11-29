using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTANetworkAPI;
using Newtonsoft.Json.Linq;

namespace ServerSide.Scripts.DrivingSchool
{
    public class DrivingSchool : Script
    {
        private readonly Vector3 _drivingSchoolPos = new Vector3(1160.2335, -456.6353, 66.98436);
        private JObject _vehicleSpawnLocations;
        private JObject _drivingTest;
        private Dictionary<int, DrivingSchoolUser> _userPool = new Dictionary<int, DrivingSchoolUser>();
        private int _vehicleIndex;

        [ServerEvent(Event.ResourceStart)]
        public void OnStart()
        {
            NAPI.Marker.CreateMarker(
                0,
                _drivingSchoolPos,
                new Vector3(),
                new Vector3(),
                0.5f,
                255, 0, 0,
                true,
                0);
            
            _vehicleSpawnLocations = NAPI.Util.FromJson(File.ReadAllText("dotnet\\resources\\GTARoleplay\\ServerSide\\Scripts\\DrivingSchool\\VehicleSpawns.json"));
            _drivingTest = NAPI.Util.FromJson(File.ReadAllText("dotnet\\resources\\GTARoleplay\\ServerSide\\Scripts\\DrivingSchool\\DrivingTest.json"));
            
            if (_vehicleSpawnLocations["Locations"] != null) _vehicleIndex = _vehicleSpawnLocations["Locations"].Count() - 1;
        }

        [RemoteEvent("DrivingSchool:Start")]
        public void EVT_DrivingSchoolStart(Player player)
        {
            if (player.Position.Subtract(_drivingSchoolPos).Length() > 2) return;
            if (_userPool.ContainsKey(player.Id))
            {
                DrivingSchoolUser schoolUser = _userPool[player.Id];
                
                schoolUser.StopSession();
                _userPool.Remove(player.Id);
                return;
            }

            string model = (string) _drivingTest["Car"]?["VehicleModel"];
            _userPool.Add(player.Id, new DrivingSchoolUser(model));

            CreateVehicle(player);
            LoadCheckpoint(player);
            NAPI.ClientEvent.TriggerClientEvent(player, "DrivingSchool:Test:StartTimer");
        }

        [RemoteEvent("DrivingSchool:ReachedCheckpoint")]
        public void EVT_DrivingSchoolCheckpoint(Player player)
        {
            DrivingSchoolUser schoolUser = _userPool[player.Id];

            if (_drivingTest["Car"]?["Checkpoints"]?[schoolUser.Checkpoint] == null) return;
            player.SendChatMessage((string) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["CheckpointInfo"]);

            schoolUser.Checkpoint++;

            if (_drivingTest["Car"]["Checkpoints"].Count() == schoolUser.Checkpoint)
            {
                player.SendChatMessage(schoolUser.SpeedingOffences < (int) _drivingTest["Car"]["MaxSpeedingOffences"]
                    ? "You have successfully passed your driving test"
                    : "You have failed your driving test");
                
                schoolUser.StopSession();
                _userPool.Remove(player.Id);
                NAPI.ClientEvent.TriggerClientEvent(player, "DrivingSchool:Test:StopTimer");
                return;
            }
            
            LoadCheckpoint(player);
            }

        [RemoteEvent("DrivingSchool:SpeedCheck")]
        public void EVT_DrivingSchoolSpeedCheck(Player player, double speed)
        {
            DrivingSchoolUser schoolUser = _userPool[player.Id];
            
            if (_drivingTest["Car"]?["Checkpoints"]?[schoolUser.Checkpoint] == null) return;
            
            if ((int) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["MaxSpeed"] < speed)
            {
                schoolUser.SpeedingOffences++;
                player.SendChatMessage("Slow down! You are driving too fast.");
            }
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void EVT_PlayerExitDrivingSchoolVeh(Player player, Vehicle vehicle)
        {
            if (_userPool.ContainsKey(player.Id))
            {
                DrivingSchoolUser schoolUser = _userPool[player.Id];
                Vehicle veh = NAPI.Pools.GetAllVehicles().Find(veh => veh.Handle == vehicle.Handle);

                if (veh != null && player.IsInVehicle)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "DrivingSchool:Test:StopTimer");
                    schoolUser.StopSession();
                    _userPool.Remove(player.Id);

                    player.SendChatMessage("You will need to redo the test again, next time don't leave the vehicle.");
                }
            }
        }

        private void CreateVehicle(Player player)
        {
            if (_vehicleSpawnLocations["Locations"]?[_vehicleIndex] == null) return;
            
            DrivingSchoolUser schoolUser = _userPool[player.Id];

            Vector3 spawnLocation = new Vector3(
                (float) _vehicleSpawnLocations["Locations"][_vehicleIndex]["x"],
                (float) _vehicleSpawnLocations["Locations"][_vehicleIndex]["y"],
                (float) _vehicleSpawnLocations["Locations"][_vehicleIndex]["z"]
            );

            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(schoolUser.VehicleHash, spawnLocation, 225, 5, 5, "STUDENT");
            player.SetIntoVehicle(vehicle, 0);
            
            schoolUser.StartSession(vehicle);
            _vehicleIndex--;
        }

        private void LoadCheckpoint(Player player)
        {
            DrivingSchoolUser schoolUser = _userPool[player.Id];

            if (_drivingTest["Car"]?["Checkpoints"]?[schoolUser.Checkpoint]?["Position"] == null) return;
            if (_drivingTest["Car"]?["Checkpoints"]?[schoolUser.Checkpoint]?["Heading"] == null) return;

            int totalCheckpoints = _drivingTest["Car"]["Checkpoints"].Count() - 1;

            if (schoolUser.Checkpoint <= totalCheckpoints)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "DrivingSchool:RenderCheckpoint", 
                    (float) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["Position"]["x"],
                    (float) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["Position"]["y"],
                    (float) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["Position"]["z"],
                    (float) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["Heading"]["x"],
                    (float) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["Heading"]["y"],
                    (float) _drivingTest["Car"]["Checkpoints"][schoolUser.Checkpoint]["Heading"]["z"]
                    );
            }
        }
    }
}