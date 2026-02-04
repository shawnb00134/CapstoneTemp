namespace CAMCMSServer.Model;

public class PackageAndOrganizations
{
    #region Properties

    public int PackageId { get; set; }
    public IEnumerable<int> OrganizationIds { get; set; }

    #endregion
}