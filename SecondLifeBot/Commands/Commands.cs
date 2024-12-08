using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibreMetaverse;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class Commands
    {
        private readonly GridClient client;
        private readonly List<UUID> adminList;
        private readonly BotManager botManager;
        private readonly ObjectScanner movementScanner;
        public Commands(GridClient client, List<UUID> adminList, BotManager botManager, ObjectScanner movementScanner)
        {
            this.client = client;
            this.adminList = adminList;
            this.botManager = botManager;
            this.movementScanner = movementScanner;
            this.client.Self.IM += HandleIM;
            this.client.Objects.ObjectUpdate += async (sender, e) => await HandleObjectUpdateAsync(sender, e);
        }

        private void HandleIM(object sender, InstantMessageEventArgs e)
        {
            if (this.adminList.Contains(e.IM.FromAgentID))
            {
                Logger.C($"Unauthorized IM from {e.IM.FromAgentName}. Ignoring.", Logger.MessageType.Alert);
                return;
            }

            if (e.IM.Dialog == InstantMessageDialog.RequestTeleport)
            {
                Logger.C($"Received teleport invite from {e.IM.FromAgentName}. Accepting invite...", Logger.MessageType.Info);
                this.client.Self.TeleportLureRespond(e.IM.FromAgentID, e.IM.IMSessionID, true);
                Logger.C("Teleport invite accepted.", Logger.MessageType.Regular);
                return;
            }

            string message = e.IM.Message.Trim().ToLower();

            switch (message)
            {
                default:
                    Logger.C($"Unknown command received: {message}", Logger.MessageType.Info);
                    break;
            }
        }

        private async Task HandleObjectUpdateAsync(object sender, PrimEventArgs e)
        {
            if (e.Prim == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                string hoverText = e.Prim.Text;
                if (!string.IsNullOrEmpty(hoverText))
                {
                    if (hoverText.IndexOf("with a score of", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Logger.C($"Matching Object Found: Name: {e.Prim.Properties?.Name}, Hover Text: '{hoverText}', ID: {e.Prim.ID}", Logger.MessageType.Alert);

                    }
                }
                else
                {
                }
            });
        }
    }
}
