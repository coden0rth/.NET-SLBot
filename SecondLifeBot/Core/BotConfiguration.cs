using System.Collections.Generic;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class BotConfiguration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public List<Vector3> PatrolPoints { get; set; } = new List<Vector3>();
        public List<UUID> AdminList { get; set; } = new List<UUID>();
        public List<string> SearchHoverText { get; set; } = new List<string>();
    }
}
