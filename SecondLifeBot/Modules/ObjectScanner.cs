using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMetaverse;

namespace SecondLifeBot.Modules
{
    public class ObjectScanner
    {
        private readonly GridClient _client;
        private readonly Movement _movement;
        private readonly List<Vector3> _patrolPoints;
        private int _currentPatrolIndex;
        private bool _stopPatrol;
        private readonly List<string> _searchCriteria;

        public static EventHandler<string> AlertDetection;
        public ObjectScanner(GridClient client, Movement movement, List<Vector3> patrolPoints, List<string> hoverTextSearch)
        {
            _client = client;
            _movement = movement;
            _patrolPoints = patrolPoints;
            _currentPatrolIndex = 0;
            _stopPatrol = false;
            _searchCriteria = hoverTextSearch;

            _client.Self.Movement.Camera.Far = 1024f;
            _client.Self.Movement.SendUpdate();
        }

        public async Task StartPatrolAsync()
        {
            Logger.C("Starting patrol...", Logger.MessageType.Info);

            while (!_stopPatrol)
            {
                if (_patrolPoints.Count == 0)
                {
                    Logger.C("No patrol points defined.", Logger.MessageType.Alert);
                    break;
                }

                if (_currentPatrolIndex < _patrolPoints.Count)
                {
                    Vector3 targetPosition = _patrolPoints[_currentPatrolIndex];
                    Logger.C($"Moving to patrol point {_currentPatrolIndex + 1}/{_patrolPoints.Count}: {targetPosition}", Logger.MessageType.Info);

                    await _movement.StartManualMovement(targetPosition);
                    while (Vector3.Distance(_client.Self.SimPosition, targetPosition) > 3.0f)
                    {
                        Logger.C($"({_client.Self.SimPosition.X},{_client.Self.SimPosition.Y}, {_client.Self.SimPosition.Z})");
                        await Task.Delay(500);
                    }
                    Logger.C($"Arrived at patrol point {_currentPatrolIndex + 1}/{_patrolPoints.Count}. Waiting for 5 seconds...", Logger.MessageType.Info);
                    await Task.Delay(5000); // Wait 5 seconds to let things load

                    _currentPatrolIndex++;
                }

                if (_currentPatrolIndex >= _patrolPoints.Count)
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
            if (_searchCriteria == null || _searchCriteria.Count < 1)
            {
                Logger.C("No search criteria provided. Skipping object scan.", Logger.MessageType.Warn);
                return;
            }

            Logger.C($"Scanning region for objects with hover text matching: {string.Join(", ", _searchCriteria)}", Logger.MessageType.Warn);

            var objectPrimitivesCopy = new List<Primitive>(_client.Network.CurrentSim.ObjectsPrimitives.Values);

            foreach (var prim in objectPrimitivesCopy)
            {
                if (prim == null) continue;

                foreach (var criteria in _searchCriteria)
                {
                    if (!string.IsNullOrEmpty(prim.Text) && prim.Text.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Logger.C($"Detected Object Found: Name: {prim.Properties.Name}, Hover Text: '{prim.Text}', ID: {prim.ID}, Parent ID: {prim.ParentID}", Logger.MessageType.Alert);
                        BotManager.SendIMToAdmins($"Matching Object Found: Name: {prim.Properties.Name}, Hover Text: '{prim.Text}', ID: {prim.ID}, Parent ID: {prim.ParentID}");
                    }
                }
            }
        }
    }
}