using System.Collections;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class PackageAndOrganizationsTests
{
    #region Methods

    [Test]
    public void TestGetValues()
    {
        var packagesAndOrgs = new PackageAndOrganizations
        {
            PackageId = 2,
            OrganizationIds = new List<int> { 1, 2 }
        };

        Assert.AreEqual(packagesAndOrgs.PackageId, 2);
        Assert.Contains(1, (ICollection?)packagesAndOrgs.OrganizationIds);
        Assert.Contains(2, (ICollection?)packagesAndOrgs.OrganizationIds);
    }

    #endregion
}