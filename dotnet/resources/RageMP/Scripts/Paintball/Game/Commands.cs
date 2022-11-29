using System;
using GTANetworkAPI;

namespace ServerSide.Scripts.Paintball.Game
{
    public class Commands : Script
    {

        [Command("paintball")]
        public void CMD_PaintballUtils(Player player, string subCommand, string arg1 = "", string arg2 = "", string arg3 = "")
        {
            if (subCommand.Length == 0)
            {
                player.SendChatMessage("Please add some arguments");
                return;
            }

            switch (subCommand)
            {
                case "setspawn":
                    SetSpawn(player);
                    break;
                case "stopgame":
                    StopGame(player, short.Parse("4"));
                    break;
            }
        }

        private void SetSpawn(Player player)
        {
            player.SendChatMessage("Spawn set");
        }
        
        private void StopGame(Player player, int gameId)
        {
            player.SendChatMessage("Game stopped");
        }
        
    }
}