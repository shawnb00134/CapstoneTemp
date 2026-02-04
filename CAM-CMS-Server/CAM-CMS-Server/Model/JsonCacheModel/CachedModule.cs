namespace CAMCMSServer.Model.JsonCacheModel
{
    public class CachedModule
    {
        #region Data members

        public string title { get; set; }

        public List<List<CacheElementSet>> pages { get; set; }

        public List<CacheElementSet> elementSets { get; set; }

        public List<CacheAnchor> anchors { get; set; }


        #endregion


    }
}
