using System;
using System.Threading.Tasks;
using OpenMetaverse;

namespace SecondLifeBot.Modules
{
    public static class ChatCommands
    {
        public static async Task ProcessIMCommandAsync(UUID fromAgentID, string message)
        {
            switch (message)
            {
                case "walktome":
                    Logger.C("Processing 'cometome' command.", Logger.MessageType.Info);
                    await Task.Run(() => BotManager.WalkToPlayer(fromAgentID));
                    break;
                case "teletome":
                    Logger.C("Processing 'teleporttome' command.", Logger.MessageType.Info);
                    await Task.Run(() => BotManager.TeleportToAdmin(fromAgentID));
                    break;
               
            }
        }
    }
}
