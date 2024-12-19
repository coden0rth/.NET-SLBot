using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibreMetaverse;
using OpenMetaverse;
using SecondLifeBot.Modules;

namespace SecondLifeBot
{
    class Program
    {
        static readonly GridClient client = new GridClient();
        static LoginManager loginManager;


        static bool running = true;
        static bool avatarLoaded = false;

        static async Task Main()
        {
            Logger.Init();
            BotManager.Initialize(client);
            loginManager = new LoginManager(client);
            Settings.LOG_LEVEL = Helpers.LogLevel.None;
            client.Network.Disconnected += Network_Disconnected;
            client.Network.EventQueueRunning += Network_EventQueueRunning;

            loginManager.Login(BotManager.Config.FirstName, BotManager.Config.LastName, BotManager.Config.Password);

            while (running)
            {
                if (avatarLoaded)
                {
                    avatarLoaded = false;
                    BotManager.Teleport(BotManager.Config.StartRegion, BotManager.Config.StartPosition);
                    BotManager.Movement.StandOnGround();
                    BotManager.PlayerScanner.StartScanning();


                }

                await Task.Delay(100);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void Network_EventQueueRunning(object sender, EventArgs e)
        {
            if (!avatarLoaded)
            {
                Logger.C("Event queue is running. Waiting for avatar appearance to load...", Logger.MessageType.Info);
                new Thread(CheckAvatarLoaded).Start();
            }
        }

        private static void CheckAvatarLoaded()
        {
            int attempts = 0;
            int maxAttempts = 20;

            while (attempts < maxAttempts)
            {
                var myAvatar = client.Network.CurrentSim.ObjectsAvatars
                    .FirstOrDefault(a => a.Value.ID == client.Self.AgentID).Value;

                if (myAvatar != null)
                {
                    Logger.C("Avatar is now present in the region. Ready for commands.", Logger.MessageType.Regular);
                    avatarLoaded = true;
                    return;
                }

                Thread.Sleep(1000);
                attempts++;
            }

            Logger.C("Failed to detect avatar presence within the time limit.", Logger.MessageType.Alert);
            running = false;
        }

        private static void Network_Disconnected(object sender, DisconnectedEventArgs e)
        {
            Logger.C("Disconnected from the grid.", Logger.MessageType.Alert);
            running = false;
        }
    }
}
