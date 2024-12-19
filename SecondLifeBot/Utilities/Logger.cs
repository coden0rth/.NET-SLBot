using System;
using System.Text;

namespace SecondLifeBot
{
    public static class Logger
    {
        public enum MessageType
        {
            Regular,
            Info,
            Warn,
            Alert,
            Chat
        }
        public static void C(string message, MessageType type = MessageType.Regular)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            string timeStamp = DateTime.Now.ToString("HH:mm:ss");
            string prefix = $"[SLB][{timeStamp}]";

            switch (type)
            {
                case MessageType.Alert:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{prefix} [ALERT] {message}");
                    break;

                case MessageType.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{prefix} [INFO] {message}");
                    break;

                case MessageType.Regular:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{prefix} {message}");
                    break;

                case MessageType.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{prefix} [WARN] {message}");
                    break;

                case MessageType.Chat:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{prefix} [IM] {message}");
                    break;
            }

            Console.ForegroundColor = originalColor;
        }

        public static void Init()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            string appName = "SecondLifeBot";
            string author = "Kyle Rambeau - coden0rth";
            string version = "Version 1.0";
            string description = "An automation bot for Duck Seoyung";

            string border = new string('=', 50);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(border);
            Console.WriteLine(CenterText($"Welcome to {appName}", 50));
            Console.WriteLine(CenterText($"Created by {author}", 50));
            Console.WriteLine(CenterText(version, 50));
            Console.WriteLine(CenterText(description, 50));
            Console.WriteLine(border);
            Console.ForegroundColor = originalColor;
        }

        private static string CenterText(string text, int width)
        {
            if (text.Length >= width) return text;
            int padding = (width - text.Length) / 2;
            return new string(' ', padding) + text + new string(' ', padding);
        }
    }
}
