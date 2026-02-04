using Newtonsoft.Json.Linq;

namespace CAMCMSServer.Model.JsonCacheModel
{
    public class CacheElementSet
    {
        public int setLocationId { get; set; }

        public int place { get; set; }

        public JObject? styling { get; set; }

        public List<CacheLocation> locations { get; set; }
    }
}
