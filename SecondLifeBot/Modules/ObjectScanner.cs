using OpenMetaverse;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecondLifeBot.Modules
{
    public class ObjectScanner
    {
        private readonly GridClient _client;
        private int _currentPatrolIndex;
        private bool _stopPatrol;

        public static EventHandler<string> AlertDetection;
        public ObjectScanner(GridClient client)
        {
            _client = client;
            _currentPatrolIndex = 0;
            _stopPatrol = false;

            _client.Self.Movement.Camera.Far = 1024f;
            _client.Self.Movement.SendUpdate();
        }

        public async Task StartPatrolAsync()
        {
            Logger.C("Starting patrol...", Logger.MessageType.Info);

            while (!_stopPatrol)
            {
                if (BotManager.Config.PatrolPoints.Count == 0)
                {
                    Logger.C("No patrol points defined.", Logger.MessageType.Alert);
                    break;
                }

                if (_currentPatrolIndex < BotManager.Config.PatrolPoints.Count)
                {
                    Vector3 targetPosition = BotManager.Config.PatrolPoints[_currentPatrolIndex];
                    Logger.C($"Moving to patrol point {_currentPatrolIndex + 1}/{BotManager.Config.PatrolPoints.Count}: {targetPosition}", Logger.MessageType.Info);

                    await BotManager.Movement.StartManualMovement(targetPosition);
                    while (Vector3.Distance(_client.Self.SimPosition, targetPosition) > 3.0f)
                    {
                        Logger.C($"({_client.Self.SimPosition.X},{_client.Self.SimPosition.Y}, {_client.Self.SimPosition.Z})");
                        await Task.Delay(500);
                    }
                    Logger.C($"Arrived at patrol point {_currentPatrolIndex + 1}/{BotManager.Config.PatrolPoints.Count}. Waiting for 5 seconds...", Logger.MessageType.Info);
                    await Task.Delay(5000); // Wait 5 seconds to let things load

                    _currentPatrolIndex++;
                }

                if (_currentPatrolIndex >= BotManager.Config.PatrolPoints.Count)
                {
                    Logger.C("Restarting patrol loop.", Logger.MessageType.Regular);
                    _currentPatrolIndex = 0;
                }

                await Task.Run(() => ScanRegionForObjects());

                await Task.Delay(1000); // Delay before the next patrol point
            }

            Logger.C("Patrol stopped.", Logger.MessageType.Info);
        }

        public void StopPatrol()
        {
            _stopPatrol = true;
        }

        public void RestartPatrol()
        {
            Logger.C("Restarting patrol after external command.", Logger.MessageType.Info);
            _stopPatrol = false;
            _currentPatrolIndex = 0;
            Task.Run(StartPatrolAsync); // Restart patrol asynchronously
        }

        public void ScanRegionForObjects()
        {
            if (BotManager.Config.SearchHoverText == null || BotManager.Config.SearchHoverText.Count < 1)
            {
                Logger.C("No search criteria provided. Skipping object scan.", Logger.MessageType.Warn);
                return;
            }

            Logger.C($"Scanning region for objects with hover text matching: {string.Join(", ", BotManager.Config.SearchHoverText)}", Logger.MessageType.Warn);

            var objectPrimitivesCopy = new List<Primitive>(_client.Network.CurrentSim.ObjectsPrimitives.Values);

            foreach (var prim in objectPrimitivesCopy)
            {
                if (prim == null) continue;

                foreach (var criteria in BotManager.Config.SearchHoverText)
                {
                    if (!string.IsNullOrEmpty(prim.Text) && prim.Text.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Logger.C($"Detected Object Found: Name: {prim.Properties.Name}, Hover Text: '{prim.Text}', ID: {prim.ID}, Parent ID: {prim.ParentID}", Logger.MessageType.Alert);
                        BotManager.IM.SendIMToAdmins($"Matching Object Found: Name: {prim.Properties.Name}, Hover Text: '{prim.Text}', ID: {prim.ID}, Parent ID: {prim.ParentID}");
                    }
                }
            }
        }
    }
}