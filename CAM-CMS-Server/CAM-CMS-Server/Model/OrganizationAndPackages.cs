namespace CAMCMSServer.Model
{
    public class OrganizationAndPackages
    {
        #region Data members
        public int OrganizationId {  get; set; }
        public IEnumerable<int> PackageIds { get; set; }
        #endregion
    }
}
