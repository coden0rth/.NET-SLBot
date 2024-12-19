using OpenMetaverse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecondLifeBot.Modules
{
    public class IM
    {
        private readonly GridClient _client;
        private readonly List<UUID> _adminList;

        public IM(GridClient client)
        {
            _client = client;
            _adminList = BotManager.Config.AdminList;

            _client.Self.IM += async (sender, e) => await HandleIMAsync(sender, e);
            _client.Friends.FriendshipOffered += async (sender, e) => await HandleFriendshipOfferedAsync(sender, e);
        }

        private async Task HandleIMAsync(object sender, InstantMessageEventArgs e)
        {
            Logger.C($"[{e.IM.FromAgentName}]: {e.IM.Message.Trim()}", Logger.MessageType.Chat);
            if (!_adminList.Contains(e.IM.FromAgentID))
            {
                Logger.C($"Unauthorized IM from {e.IM.FromAgentName}. Ignoring.", Logger.MessageType.Alert);
                return;
            }

            if (e.IM.Dialog == InstantMessageDialog.RequestTeleport)
            {
                Logger.C($"Received teleport invite from {e.IM.FromAgentName}. Accepting invite...", Logger.MessageType.Info);
                _client.Self.TeleportLureRespond(e.IM.FromAgentID, e.IM.IMSessionID, true);
                Logger.C("Teleport invite accepted.", Logger.MessageType.Regular);
                return;
            }

            string message = e.IM.Message.Trim().ToLower();

            await ChatCommands.ProcessIMCommandAsync(e.IM.FromAgentID, message);
        }

        private async Task HandleFriendshipOfferedAsync(object sender, FriendshipOfferedEventArgs e)
        {
            if (_adminList.Contains(e.AgentID))
            {
                Logger.C($"Friendship offer received from admin {e.AgentName}. Accepting friendship...", Logger.MessageType.Info);
                await Task.Run(() => _client.Friends.AcceptFriendship(e.AgentID, e.SessionID));
                Logger.C("Friendship request accepted.", Logger.MessageType.Regular);
            }
            else
            {
                Logger.C($"Friendship offer received from {e.AgentName}, but they are not an admin. Ignoring.", Logger.MessageType.Alert);
            }
        }

        public async Task SendIMAsync(UUID agentID, string message)
        {
            await Task.Run(() => _client.Self.InstantMessage(agentID, message));
        }
        public void SendIMToAdmins(string message)
        {
            if (BotManager.Config.AdminList == null || BotManager.Config.AdminList.Count == 0)
            {
                Logger.C("Admin list is empty or null.", Logger.MessageType.Alert);
                return;
            }

            foreach (var adminUUID in BotManager.Config.AdminList)
            {
                _ = SendIMAsync(adminUUID, message);
            }

            Logger.C($"Message sent to all admins: {message}", Logger.MessageType.Info);
        }
    }
}
