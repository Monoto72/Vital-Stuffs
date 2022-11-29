using System.Collections.Generic;
using System.IO;
using GTANetworkAPI;
using Newtonsoft.Json.Linq;

namespace ServerSide.Scripts.Paintball.Game
{
    public class GameInstance : Script
    {
        public int SessionHost { get; set; }
        public string MapName { set; get; }
        public int ModeType { set; get; }
        public int MaxPlayers { set; get; }
        public int MaxScore { set; get; }
        public string SessionPassword { set; get; }


        public GameInstance() {}
    }
}