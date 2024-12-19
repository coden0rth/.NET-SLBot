using System;
using System.Collections.Generic;
using OpenMetaverse;
using System.Threading;
using System.Threading.Tasks;
using SecondLifeBot.Modules;


namespace SecondLifeBot
{
    public static class BotManager
    {
        private static GridClient _client;
        public static BotConfiguration Config;
        public static Movement Movement;
        public static PlayerScanner PlayerScanner;
        public static ObjectScanner ObjectScanner;
        public static IM IM;



        public static void Initialize(GridClient client)
        {
            Logger.C("Loading configuration...", Logger.MessageType.Info);
            Config = Loader.LoadConfiguration("Config/config.json");
            Logger.C("Configuration loaded successfully.", Logger.MessageType.Regular);

            _client = client;
            _client.Self.Movement.Camera.Far = 1024f;
            _client.Self.Movement.SendUpdate();

            Logger.C("Loading Movement module...", Logger.MessageType.Info);
            Movement = new Movement(_client);
            Logger.C("Movement module loaded successfully.", Logger.MessageType.Regular);

            Logger.C("Loading IM module...", Logger.MessageType.Info);
            IM = new IM(_client);
            Logger.C("IM module loaded successfully.", Logger.MessageType.Regular);

            Logger.C("Loading ObjectScanner module...", Logger.MessageType.Info);
            ObjectScanner = new ObjectScanner(_client);
            Logger.C("ObjectScanner module loaded successfully.", Logger.MessageType.Regular);

            Logger.C("Loading PlayerScanner module...", Logger.MessageType.Info);
            PlayerScanner = new PlayerScanner(_client);
            PlayerScanner.OnPlayerJoin += OnPlayerJoin;
            PlayerScanner.OnPlayerLeave += OnPlayerLeave;
            Logger.C("PlayerScanner module loaded successfully.", Logger.MessageType.Regular);
        }


        private static void OnPlayerJoin(Avatar avatar)
        {
            Logger.C($"Welcome {avatar.Name} to the region!", Logger.MessageType.Regular);
        }
        private static void OnPlayerLeave(UUID playerId)
        {
            Logger.C($"Player with ID {playerId} has left the region.", Logger.MessageType.Regular);

        }
        public static Avatar LookupAvatar(UUID playerUUID)
        {
            foreach (var avatar in _client.Network.CurrentSim.ObjectsAvatars.Values)
            {
                if (avatar.ID == playerUUID)
                {
                    return avatar;
                }
            }

            return null;
        }
        public static async Task WalkToPlayer(UUID playerUUID)
        {
            Avatar targetAvatar = LookupAvatar(playerUUID);

            if (targetAvatar != null)
            {
                Vector3 playerPosition = targetAvatar.Position;
                Logger.C($"Walking to player at position: {playerPosition}", Logger.MessageType.Info);

                await Movement.StartManualMovement(playerPosition);
            }
            else
            {
                Logger.C("Player not found in the current region.", Logger.MessageType.Alert);
            }
        }
        public static void TeleportToAdmin(UUID adminID)
        {
            Avatar adminAvatar = LookupAvatar(adminID);

            if (adminAvatar != null)
            {
                Vector3 adminPosition = adminAvatar.Position;
                string regionName = _client.Network.CurrentSim.Name;

                Logger.C($"Teleporting to admin {adminAvatar.Name} at {regionName} ({adminPosition})", Logger.MessageType.Info);

                _client.Self.Teleport(regionName, adminPosition);
                Logger.C("Teleport successful.", Logger.MessageType.Regular);
            }
            else
            {
                Logger.C("Admin not found in the current region. Cannot teleport.", Logger.MessageType.Alert);
            }
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
       
    }
}
