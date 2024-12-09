using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LibreMetaverse;
using OpenMetaverse;

namespace SecondLifeBot
{
    class Program
    {
        static readonly GridClient client = new GridClient();
        static LoginManager loginManager;
        static BotManager botManager;
        static ObjectScanner movementScanner;
        static Commands commands;
        static Movement movement;

        static BotConfiguration config;

        static bool running = true;
        static bool avatarLoaded = false;

        static async Task Main()
        {
            Logger.Init();
            config = Loader.LoadConfiguration("Config/config.json");
            botManager = new BotManager(client);
            movement = new Movement(client);
            movementScanner = new ObjectScanner(client, movement, config.PatrolPoints, config.SearchHoverText);
            commands = new Commands(client, config.AdminList, botManager, movementScanner);
            loginManager = new LoginManager(client);

            Settings.LOG_LEVEL = Helpers.LogLevel.None;
            client.Network.Disconnected += Network_Disconnected;
            client.Network.EventQueueRunning += Network_EventQueueRunning;

            loginManager.Login(config.FirstName, config.LastName, config.Password);

            while (running)
            {
                if (avatarLoaded)
                {
                    avatarLoaded = false;
                    botManager.Teleport(config.StartRegion, config.StartPosition);
                    await movementScanner.StartPatrolAsync();
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
                Logger.C("Event queue is running. Waiting for avatar appearance to load...", Logger.MessageType.Warn);
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
