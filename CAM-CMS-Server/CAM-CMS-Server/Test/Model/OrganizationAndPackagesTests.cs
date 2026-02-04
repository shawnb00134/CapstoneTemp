using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model
{
    public class OrganizationAndPackagesTests
    {
        [Test]
        public void TestGetValues()
        {
            var orgAndPackages = new OrganizationAndPackages()
            {
                OrganizationId = 2,
                PackageIds = new List<int>() { 1, 2 },
            };

            Assert.AreEqual(orgAndPackages.OrganizationId, 2);
            Assert.Contains(1, (System.Collections.ICollection?)orgAndPackages.PackageIds);
            Assert.Contains(2, (System.Collections.ICollection?)orgAndPackages.PackageIds);
        }
    }
}
