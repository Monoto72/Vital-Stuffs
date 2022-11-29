using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Newtonsoft.Json.Linq;

namespace ServerSide.Scripts.Paintball.Game
{
    public class GameHandler : Script
    {
        private HashSet<GameInstance> currentGames = new HashSet<GameInstance>();

        public void CreateGame(int playerId, string map, int mode, int maxPlayers, int maxScore, string password)
        {
            NAPI.Util.ConsoleOutput("CreateGame Map:" + map + " mode:" + mode + " max-players:" + maxPlayers + "total-score:" + maxScore + " password:" + password);
            GameInstance game = new GameInstance
            {
                SessionHost = playerId,
                MapName = map,
                ModeType = mode,
                MaxPlayers = maxPlayers,
                MaxScore = maxScore,
                SessionPassword = password
            };

            currentGames.Add(game);
        }

        public HashSet<GameInstance> GetGames()
        {
            return currentGames;
        }

        public JObject GetGamesAsJson()
        {
            JObject games = new JObject();
            int count = 0;

            foreach (GameInstance game in currentGames)
            {
                games.Add("Instance-" + count, GetGameAsJson(game));
                count++;
            }

            return games;
        }
        
        private static JObject GetGameAsJson(GameInstance instance)
        {
            return new JObject(
                new JProperty("Session-Host", instance.SessionHost),
                new JProperty("Map-Name", instance.MapName),
                new JProperty("Mode-Type", instance.ModeType),
                new JProperty("Max-Players", instance.MaxPlayers),
                new JProperty("Session-Password", instance.SessionHost),
                new JProperty("Max-Score", instance.MaxScore)
            );
        }
    }
}