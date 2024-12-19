using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SecondLifeBot
{
    public static class Loader
    {
        public static BotConfiguration LoadConfiguration(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Logger.C("Configuration file could not be found or opened. Press any key to exit...", Logger.MessageType.Alert);
                Console.ReadLine();
                Task.Delay(2000);
                Environment.Exit(0);
            }

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<BotConfiguration>(json);
        }
    }
}
