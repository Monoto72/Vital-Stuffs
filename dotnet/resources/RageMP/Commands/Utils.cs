using GTANetworkAPI;

namespace ServerSide.Commands
{
    public class Utils : Script
    {
        [Command("pos")]
        public void CMD_CurrentPosition(Player player)
        {
            NAPI.Util.ConsoleOutput("Position: { \"x\": " + player.Position.X + ", \"y\": " + player.Position.Y + ", \"z\": " + player.Position.Z + " }");
        }
        
        [Command("q")]
        public void CMD_Disconnect(Player player)
        {
            player.Kick();
        }
    }
}