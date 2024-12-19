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
                case "cometome":
                    Logger.C("Processing 'cometome' command.", Logger.MessageType.Info);
                    await Task.Run(() => BotManager.WalkToPlayer(fromAgentID));
                    break;
                default:
                    Logger.C($"Unknown command received: {message}", Logger.MessageType.Info);
                    break;
            }
        }
    }
}
