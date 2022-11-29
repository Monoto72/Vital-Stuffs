using System.IO;
using GTANetworkAPI;
using Newtonsoft.Json.Linq;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Events
{
    public class ConnectionEvent : Script
    {

        [ServerEvent(Event.PlayerConnected)]
        public void onPlayerConnect(Player player)
        {
            NAPI.Util.ConsoleOutput(player.Name + " has connected");
            player.SetData("admin", true);
            player.SetClothes(2, 18, 4); // Hair
            player.SetClothes(4, 26, 1); // Legs
            player.SetClothes(6, 48, 1); // Shoes
            player.SetClothes(8, 141, 1); // Undershirt
            player.SetClothes(11, 303, 2); // Top
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void onPlayerDisconnect(Player player, DisconnectionType type, string reason)
        {
            switch (type)
            {
                case DisconnectionType.Left:
                    NAPI.Util.ConsoleOutput(player.Name + " has disconnect");
                    break;

                case DisconnectionType.Timeout:
                    NAPI.Util.ConsoleOutput(player.Name + " has timed out");
                    break;

                case DisconnectionType.Kicked:
                    NAPI.Util.ConsoleOutput(player.Name + " has been kicked for " + reason);
                    break;
            }
        }
        
    }
}