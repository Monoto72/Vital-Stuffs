using System.IO;
using System.Runtime.CompilerServices;
using GTANetworkAPI;
using Newtonsoft.Json.Linq;
using ServerSide.Scripts.Paintball.Game;

namespace ServerSide.Scripts.Paintball
{
    public class Paintball : Script
    {
        private readonly Vector3 _startPos = new Vector3(-422.40567, 1134.9958, 325.85486);
        private static readonly GameHandler _gameHandler = new GameHandler();

        private JObject _locations;

        [ServerEvent(Event.ResourceStart)]
        public void OnStart()
        {
            NAPI.Marker.CreateMarker(
                0,
                _startPos,
                new Vector3(),
                new Vector3(),
                0.5f,
                255, 0, 0,
                true,
                0);
            
            _locations = NAPI.Util.FromJson(File.ReadAllText("dotnet\\resources\\GTARoleplay\\ServerSide\\Scripts\\Paintball\\Data\\Locations.json"));
        }

        [RemoteEvent("Paintball:Interact")]
        public void EVT_PaintballInteraction(Player player)
        {
            if (player.Position.Subtract(_startPos).Length() > 2) return;
            NAPI.ClientEvent.TriggerClientEvent(player, "Paintball:PromptMenu", _gameHandler.GetGamesAsJson());
        }

        [RemoteEvent("Paintball:CreateGame")]
        public void EVT_PaintballCreateGame(Player player, string map, int mode, int maxPlayers, int maxScore, string password = "")
        {
            NAPI.Util.ConsoleOutput("Event:CreateGame Map:" + map + " mode:" + mode + " max-players:" + maxPlayers + "total-score:" + maxScore + " password:" + password);
            _gameHandler.CreateGame(player.Id, map, mode, maxPlayers, maxScore, password);
            NAPI.Util.ConsoleOutput(_gameHandler.GetGamesAsJson().ToString());
        }

        public static GameHandler GetGameHandler()
        {
            return _gameHandler;
        }
    }
}