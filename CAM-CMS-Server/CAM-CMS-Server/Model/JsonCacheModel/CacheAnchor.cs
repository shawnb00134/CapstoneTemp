namespace CAMCMSServer.Model.JsonCacheModel
{
    public class CacheAnchor
    {
        public int anchorId { get; set; }

        public int page { get; set; }

        public string content { get; set; }

        public bool subHeader { get; set; }

        public int level { get; set; }

        public List<CacheAnchor> headerChildren { get; set; }

        public CacheAnchor? headerParent { get; set; }
    }
}
