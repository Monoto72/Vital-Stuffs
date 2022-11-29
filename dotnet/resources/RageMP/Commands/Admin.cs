using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTANetworkAPI;
using Newtonsoft.Json.Linq;

namespace ServerSide.Commands
{
    public class Admin : Script
    {
        
        [Command("vehicle", Alias = "veh")]
        public void CMD_SpawnVehicle(Player player, string model)
        {

            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to Spawn a Vehicle outside of staff-mode");
                return;
            }

            if (player.GetData<bool>("adminVehicleSpawned"))
            {
                int currentVehicleId = player.GetData<UInt16>("adminVehicleId");
                Vehicle currentVehicle = NAPI.Pools.GetAllVehicles().Find(vehicle => vehicle.Id == currentVehicleId);

                if (currentVehicle != null)
                {
                    currentVehicle.Delete();
                }
            }
            
            VehicleHash hash = NAPI.Util.VehicleNameToModel(model);
            Vector3 position = new Vector3(
                player.Position.X + Math.Sin(-player.Heading * Math.PI / 180) * 3,
                player.Position.Y + Math.Cos(-player.Heading * Math.PI / 180) * 3,
                player.Position.Z - 0.98);
            
            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(hash, position, player.Heading, 0, 0);

            NAPI.Util.ConsoleOutput(player.Id + " has spawned in a Non-Roleplay Vehicle");
            player.SetData("adminVehicleSpawned", true);
            player.SetData("adminVehicleId", vehicle.Id);
            player.SetIntoVehicle(vehicle, 0);
        }

        [Command("deleteveh", Alias = "delveh")]
        public void CMD_DeleteVehicle(Player player, int id)
        {
            
            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to Delete a Vehicle outside of staff-mode");
                return;
            }

            //No DB just yet, so just delete set total to 0
            player.SetData("VehicleTotal", 0);
        }

        [Command("fixcar")]
        public void CMD_FixVehicle(Player player)
        {

            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to Fix a Vehicle outside of staff-mode");
                return;
            }
            
            if (player.Vehicle != null)
            {
                player.Vehicle.Repair();
                NAPI.Util.ConsoleOutput(player.Id + " has fixed Vehicle: " + player.Vehicle.Id);
            }
            else
            {
                player.SendChatMessage("You need to be inside a vehicle to fix it");
            }
        }

        [Command("vehrange", Alias = "vrange")]
        public void CMD_ListVehiclesInRange(Player player, int range)
        {
            
            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to get all Vehicles within Range outside of staff-mode");
                return;
            }

            List<Vehicle> vehiclesInRange = NAPI.Pools.GetAllVehicles()
                .FindAll(vehicle => vehicle.Position.Subtract(player.Position).Length() <= range);

            if (vehiclesInRange.Any())
            {
                foreach (Vehicle vehicle in vehiclesInRange)
                {
                    player.SendChatMessage(vehicle.Id + ": " + vehicle.DisplayName);
                }
            }
            else
            {
                player.SendChatMessage("No vehicles were found within a " + range + "m range");
            }
        }

        [Command("flipcar")]
        public void CMD_FlipVehicle(Player player, int id)
        {
            
            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to Flip a Vehicle outside of staff-mode");
                return;
            } 
            
            Vehicle vehicle = NAPI.Pools.GetAllVehicles().Find(vehicle => vehicle.Id == id);

            if (vehicle != null)
            {
                vehicle.Rotation = new Vector3(0, 0, 0);
                NAPI.Util.ConsoleOutput(player.Id + " has flipped Vehicle: " + vehicle.Id);
            }
        }

        [Command("bringveh", Alias = "bringv")]
        public void CMD_TeleportVehicle(Player player, int id)
        {
            
            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to Teleport a Vehicle outside of staff-mode");
                return;
            }
            
            Vehicle vehicle = NAPI.Pools.GetAllVehicles().Find(vehicle => vehicle.Id == id);
            Vector3 position = new Vector3(
                player.Position.X + Math.Sin(-player.Heading * Math.PI / 180) * 3,
                player.Position.Y + Math.Cos(-player.Heading * Math.PI / 180) * 3,
                player.Position.Z - 0.98);

            if (vehicle != null)
            {
                vehicle.Position = position;
                NAPI.Util.ConsoleOutput(player.Id + " has teleported Vehicle: " + vehicle.Id);
            }
            else
            {
                player.SendChatMessage("Vehicle " + id + " does not exist");
            }
        }

        [Command("maxveh")]
        public void CMD_MaxVehicleMods(Player player)
        {
            if (!player.GetData<bool>("admin"))
            {
                player.SendChatMessage("You can't run this outside of staff-mode!");
                NAPI.Util.ConsoleOutput(player.SocialClubId + " tried to Max a Vehicle outside of staff-mode");
                return;
            }

            if (player.Vehicle != null) {
                player.Vehicle.SetMod(11, 3);
                player.Vehicle.SetMod(12, 2);
                player.Vehicle.SetMod(22, 0);
                player.Vehicle.SetMod(13, 2);
                player.Vehicle.SetMod(15, 3);
                player.Vehicle.SetMod(18, 0);
                player.Vehicle.Repair();

                player.Vehicle.PrimaryColor = 28;
                
                NAPI.Util.ConsoleOutput(player.Id + " has maxed Vehicle: " + player.Vehicle.Id);
            }
            else
            {
                player.SendChatMessage("You need to be inside a vehicle to max it");
            }
        }

        [Command("teleport", Alias = "tp")]
        public void CMD_PlayerTeleport(Player player, string place = null)
        {
            
            JObject teleportData = NAPI.Util.FromJson(File.ReadAllText("dotnet\\resources\\GTARoleplay\\ServerSide\\Commands\\Data\\Teleports.json"));

            if (place == null)
            {
                foreach (var location in teleportData)
                {
                    player.SendChatMessage(location.Key);
                }

                return;
            }

            if (teleportData.ContainsKey(place))
            {
                Vector3 newPosition = new Vector3(
                    (float) teleportData[place]["x"],
                    (float) teleportData[place]["y"],
                    (float) teleportData[place]["z"]);

                player.Position = newPosition;
            }
            else
            {
                player.SendChatMessage(place + " does not exist!");
            }
        }
    }
}