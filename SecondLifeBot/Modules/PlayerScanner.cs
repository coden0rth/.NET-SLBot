using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class PlayerScanner
    {
        private readonly GridClient _client;
        private readonly HashSet<UUID> _currentPlayers = new HashSet<UUID>();
        private bool _isScanning = false;
        private bool _initialScanComplete = false;

        public event Action<Avatar> OnPlayerJoin;
        public event Action<UUID> OnPlayerLeave;

        private const int ScanInterval = 5000;

        public PlayerScanner(GridClient client)
        {
            _client = client;
        }

        public void StartScanning()
        {
            if (_isScanning) return;
            _isScanning = true;
            Logger.C("Player scanning started.", Logger.MessageType.Info);
            _ = ScanLoopAsync();
        }

        public void StopScanning()
        {
            _isScanning = false;
            Logger.C("Player scanning stopped.", Logger.MessageType.Info);
        }

        private async Task ScanLoopAsync()
        {
            while (_isScanning)
            {
                await ScanForPlayersAsync();
                await Task.Delay(ScanInterval);
            }
        }

        private async Task ScanForPlayersAsync()
        {
            await Task.Run(() =>
            {
                var avatars = _client.Network.CurrentSim.ObjectsAvatars.Values;
                var newPlayers = new HashSet<UUID>();

                foreach (var avatar in avatars)
                {
                    newPlayers.Add(avatar.ID);

                    if (!_currentPlayers.Contains(avatar.ID) && _initialScanComplete)
                    {
                        Logger.C($"Player joined: {avatar.Name}", Logger.MessageType.Info);
                        OnPlayerJoin?.Invoke(avatar);
                    }
                }

                if (!_initialScanComplete)
                {
                    Logger.C("Initial player scan complete. Players currently in the region:", Logger.MessageType.Info);
                    foreach (var avatar in avatars)
                    {
                        Logger.C($" - {avatar.Name} ({avatar.ID})", Logger.MessageType.Regular);
                    }
                    _initialScanComplete = true;
                }

                var playersWhoLeft = _currentPlayers.Except(newPlayers).ToList();
                foreach (var playerId in playersWhoLeft)
                {
                    Logger.C($"Player left: {playerId}", Logger.MessageType.Info);
                    OnPlayerLeave?.Invoke(playerId);
                }

                _currentPlayers.Clear();
                _currentPlayers.UnionWith(newPlayers);
            });
        }
    }
}
