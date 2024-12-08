using System;
using System.Collections.Generic;
using System.Threading;
using LibreMetaverse;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class BotManager
    {
        private readonly GridClient _client;
        private readonly HashSet<UUID> _adminList = new HashSet<UUID>
        {
            new UUID("1f0ee634-151d-41b6-afab-89ea0d7d0784") 
        };

        public BotManager(GridClient client)
        {
            _client = client;
            _client.Self.IM += Self_IM;
            _client.Objects.ObjectUpdate += Objects_ObjectUpdate;
            _client.Objects.ObjectPropertiesFamily += Objects_ObjectPropertiesFamily;
        }

        private void Self_IM(object sender, InstantMessageEventArgs e)
        {
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

            if (e.IM.Message.Trim().ToLower() == "scan region")
            {
                ScanRegionForObjects();
            }
        }

        public void Teleport(string regionName, Vector3 position)
        {
            if (_client.Network.Connected && _client.Self != null)
            {
                Logger.C($"Teleporting to {regionName} at {position}...", Logger.MessageType.Info);

                try
                {
                    _client.Self.Teleport(regionName, position);
                    Logger.C("Teleport request sent.", Logger.MessageType.Regular);
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

        public void ScanRegionForObjects()
        {
            Logger.C("Scanning region for objects...", Logger.MessageType.Info);

            var objectPrimitivesCopy = new List<Primitive>(_client.Network.CurrentSim.ObjectsPrimitives.Values);

            foreach (var prim in objectPrimitivesCopy)
            {
                if (prim != null)
                {
                    Logger.C($"Requesting properties for Object ID: {prim.ID}", Logger.MessageType.Info);
                    _client.Objects.RequestObjectPropertiesFamily(_client.Network.CurrentSim, prim.ID);
                }
            }
        }


        private void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
            if (e.Prim == null)
            {
                Logger.C("Received a null Prim object.", Logger.MessageType.Alert);
                return;
            }

            Logger.C($"Object update received: ID: {e.Prim.ID}, Position: {e.Prim.Position}", Logger.MessageType.Info);

            if (e.Prim.Properties == null)
            {
                Logger.C($"Requesting properties for Object ID: {e.Prim.ID}", Logger.MessageType.Info);
                _client.Objects.RequestObjectPropertiesFamily(_client.Network.CurrentSim, e.Prim.ID);
                return;
            }

            LogObjectDetails(e.Prim);
        }

        private void Objects_ObjectPropertiesFamily(object sender, ObjectPropertiesFamilyEventArgs e)
        {
            Logger.C($"Received properties for Object ID: {e.Properties.ObjectID}", Logger.MessageType.Info);
            Logger.C($"Name: {e.Properties.Name}, Description: {e.Properties.Description}, Owner: {e.Properties.OwnerID}", Logger.MessageType.Regular);
        }

        private void LogObjectDetails(Primitive prim)
        {
            Logger.C($"Object found: Name: {prim.Properties.Name}, ID: {prim.ID}, Position: {prim.Position}", Logger.MessageType.Info);
        }
    }
}
