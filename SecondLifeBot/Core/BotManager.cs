using System;
using System.Collections.Generic;
using OpenMetaverse;
using System.Threading;


namespace SecondLifeBot
{
    public static class BotManager
    {
        private static GridClient _client;
        private static BotConfiguration _config;

        public static void Initialize(GridClient client, BotConfiguration config)
        {
            _client = client;
            _client.Self.Movement.Camera.Far = 1024f;
            _client.Self.Movement.SendUpdate();
            _config = config;
        }
        public static void WalkToPlayer(UUID playerUUID)
        {
            Avatar targetAvatar = null;

            foreach (var avatar in _client.Network.CurrentSim.ObjectsAvatars.Values)
            {
                if (avatar.ID == playerUUID)
                {
                    targetAvatar = avatar;
                    break;
                }
            }

            if (targetAvatar != null)
            {
                Vector3 playerPosition = targetAvatar.Position;
                Logger.C($"Walking to player at position: {playerPosition}", Logger.MessageType.Info);

                StandOnGround();

                _client.Self.Movement.TurnToward(playerPosition);
                _client.Self.Movement.AtPos = true;

                while (Vector3.Distance(_client.Self.SimPosition, playerPosition) > 2.0f)
                {
                    _client.Self.Movement.SendUpdate();
                    System.Threading.Thread.Sleep(100);
                }

                _client.Self.Movement.AtPos = false;
                _client.Self.Movement.SendUpdate();
                Logger.C("Arrived at the player's location.", Logger.MessageType.Info);
            }
            else
            {
                Logger.C("Player not found in the current region.", Logger.MessageType.Alert);
            }
        }


        public static void StandOnGround()
        {
            if (_client.Self.Movement.Fly)
            {
                Logger.C("Bot is currently flying. Stopping fly mode...", Logger.MessageType.Info);
                _client.Self.Movement.Fly = false;
                _client.Self.Movement.SendUpdate();
            }

            if (_client.Self.SittingOn != 0)
            {
                Logger.C("Bot is sitting. Standing up...", Logger.MessageType.Info);
                _client.Self.Stand();
            }

            Logger.C("Bot is now standing on the ground.", Logger.MessageType.Regular);
        }
        public static void Teleport(string regionName, Vector3 position)
        {
            if (_client.Network.Connected && _client.Self != null)
            {
                Logger.C($"Teleporting to {regionName} at {position}...", Logger.MessageType.Info);

                try
                {
                    _client.Self.Teleport(regionName, position);
                    Logger.C("Teleport Request Completed", Logger.MessageType.Regular);
                }
                catch (Exception ex)
                {
                    Logger.C($"Teleport failed: {ex.Message}", Logger.MessageType.Alert);
                }
            }
            else
            {
                Logger.C("Cannot teleport because the bot is not connected.", Logger.MessageType.Alert);
            }
        }

        public static void SendIM(UUID targetUUID, string message)
        {
            if (targetUUID == UUID.Zero)
            {
                Logger.C("Invalid target UUID.", Logger.MessageType.Alert);
                return;
            }

            _client.Self.InstantMessage(targetUUID, message);
            Logger.C($"Sent IM to {targetUUID}: {message}", Logger.MessageType.Info);
        }

        public static void SendIMToAdmins(string message)
        {
            if (_config.AdminList == null || _config.AdminList.Count == 0)
            {
                Logger.C("Admin list is empty or null.", Logger.MessageType.Alert);
                return;
            }

            foreach (var adminUUID in _config.AdminList)
            {
                SendIM(adminUUID, message);
            }

            Logger.C($"Message sent to all admins: {message}", Logger.MessageType.Info);
        }
    }
}
