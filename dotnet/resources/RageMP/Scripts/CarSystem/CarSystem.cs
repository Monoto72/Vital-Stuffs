using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;

namespace ServerSide.CarSystem
{
    public class CarSystem : Script
    {

        [ServerEvent(Event.ResourceStart)]
        public void onStart()
        {
            NAPI.Util.ConsoleOutput("Car Resource has started");
        }

        [ServerEvent(Event.PlayerConnected)]
        public void EVT_PlayerConnected(Player player)
        {
            player.SetData("VehicleTotal", 0);
            player.SetData("VehicleLimit", 3);
            player.SetData("Vehicles", new List<Vehicle>());
        }
    }
}