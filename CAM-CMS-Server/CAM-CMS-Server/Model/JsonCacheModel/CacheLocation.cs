using Newtonsoft.Json.Linq;

namespace CAMCMSServer.Model.JsonCacheModel
{
    public class CacheLocation
    {
        public int place { get; set; }

        public Element element { get; set; }

        public JObject? locationAttribute { get; set; }


    }
}
