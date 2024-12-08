using System;
using System.Linq;
using System.Threading;
using LibreMetaverse;
using OpenMetaverse;

namespace SecondLifeBot
{
    class Program
    {
        static GridClient client = new GridClient();
        static LoginManager loginManager;
        static BotManager botManager;
        static bool running = true;
        static bool avatarLoaded = false;

        static void Main(string[] args)
        {
            Logger.Init();
            string firstName = "";
            string lastName = "Resident";
            string password = "";

            loginManager = new LoginManager(client);
            botManager = new BotManager(client);

            Settings.LOG_LEVEL = Helpers.LogLevel.None;
            client.Network.LoginProgress += Network_LoginProgress;
            client.Network.Disconnected += Network_Disconnected;
            client.Network.EventQueueRunning += Network_EventQueueRunning;
        
            loginManager.Login(firstName, lastName, password);

            while (running)
            {
                if (avatarLoaded)
                {
                    avatarLoaded = false;
                    botManager.Teleport("Krown", new Vector3(250f, 121f, 28f));
                    botManager.ScanRegionForObjects();
                }

                Thread.Sleep(100);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
        private static void Network_EventQueueRunning(object sender, EventArgs e)
        {
            Logger.C("Event queue is running. Waiting for avatar appearance to load...", Logger.MessageType.Info);
            new Thread(CheckAvatarLoaded).Start();
        }
        private static void Network_LoginProgress(object sender, LoginProgressEventArgs e)
        {
            if (e.Status == LoginStatus.Success)
            {
                
            }
            else if (e.Status == LoginStatus.Failed)
            {
                running = false;
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
                    Logger.C("Avatar is now present in the region. Ready for commands.", Logger.MessageType.Info);
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
