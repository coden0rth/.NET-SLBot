using System;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class LoginManager
    {
        private readonly GridClient _client;

        public LoginManager(GridClient client)
        {
            _client = client;
        }

        public void Login(string firstName, string lastName, string password)
        {
            LoginParams loginParams = _client.Network.DefaultLoginParams(firstName, lastName, password, "SecondLifeBot", "1.0");

            _client.Network.LoginProgress += LoginProgressHandler;
            Logger.C("Logging in...", Logger.MessageType.Warn);
            _client.Network.BeginLogin(loginParams);
        }

        private void LoginProgressHandler(object sender, LoginProgressEventArgs e)
        {
            if (e.Status == LoginStatus.Success)
            {
                Logger.C("Login successful!", Logger.MessageType.Regular);
            }
            else if (e.Status == LoginStatus.Failed)
            {
                Logger.C($"Login failed: {e.Message}", Logger.MessageType.Alert);
            }
        }
    }
}
