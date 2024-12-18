using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OpenMetaverse;
using System.Collections.Generic;

namespace SecondLifeBot
{
    public class BotConfiguration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string StartRegion { get; set; }
        public Vector3 StartPosition { get; set; }
        public bool Patroller { get; set; }
        public List<Vector3> PatrolPoints { get; set; }
        public List<string> SearchHoverText { get; set; }
        public List<UUID> AdminList { get; set; }
    }
}
