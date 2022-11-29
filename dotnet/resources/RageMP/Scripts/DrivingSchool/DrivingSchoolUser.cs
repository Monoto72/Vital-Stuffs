using System.Timers;
using GTANetworkAPI;
using GTANetworkMethods;
using Player = GTANetworkAPI.Player;
using Vehicle = GTANetworkAPI.Vehicle;

namespace ServerSide.Scripts.DrivingSchool
{
    public class DrivingSchoolUser
    {
        private NetHandle VehicleHandle { get; set; }
        public int Checkpoint { get; set; }
        public int SpeedingOffences { get; set; }

        public VehicleHash VehicleHash { get; set; }

        public DrivingSchoolUser(string model)
        {
            VehicleHash = NAPI.Util.VehicleNameToModel(model);
            SpeedingOffences = 0;
            Checkpoint = 0;
        }

        public void StartSession(Vehicle vehicle)
        {
            VehicleHandle = vehicle.Handle;
        }
        
        public void StopSession()
        {
            Vehicle vehicle = NAPI.Pools.GetAllVehicles().Find(vehicle => VehicleHandle == vehicle.Handle);
            if (vehicle != null) NAPI.Task.Run(vehicle.Delete);
            Checkpoint = 0;
        }
        
    }
}