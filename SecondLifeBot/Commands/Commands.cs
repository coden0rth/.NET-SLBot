using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class Commands
    {
        private readonly GridClient client;
        private readonly List<UUID> adminList;
        private readonly ObjectScanner objectScanner;
        private readonly BotConfiguration config;

        public Commands(GridClient client, BotConfiguration config, ObjectScanner objectScanner)
        {
            this.client = client;
            this.adminList = config.AdminList;
            this.config = config;
            this.objectScanner = objectScanner;
            this.client.Self.IM += HandleIM;
            this.client.Objects.ObjectUpdate += async (sender, e) => await HandleObjectUpdateAsync(sender, e);
        }

        private void HandleIM(object sender, InstantMessageEventArgs e)
        {
            Console.WriteLine(e.IM.FromAgentID);
            if (!adminList.Contains(e.IM.FromAgentID))
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
                    foreach (var criteria in this.config.SearchHoverText)
                    {
                        if (hoverText.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string name = e.Prim.Properties?.Name ?? "Unknown";
                            UUID id = e.Prim.ID;
                            uint parentId = e.Prim.ParentID;
                            Logger.C($"Detected Object Found: Name: {name}, Hover Text: '{hoverText}', ID: {id}", Logger.MessageType.Alert);                       
                            BotManager.SendIMToAdmins($"Matching Object Found: Name: {name}, Hover Text: '{hoverText}', ID: {id}, Parent ID: {parentId}");

                            break; 
                        }
                    }
                }
            });
        }


    }
}
