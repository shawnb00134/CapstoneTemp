using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationPackageRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
{
    #region Data members

    private IOrganizationPackageRepository organizationPackageRepository;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        this.organizationPackageRepository =
            new CAMCMSServer.Database.Repository.OrganizationPackageRepository(new MockDataContext());
    }

    [Test]
    public async Task UpdateOrganizationPackageNotImplemented()
    {
        var newOrganizationPackage = new OrganizationPackage
        {
            OrganizationId = 0,
            PackageId = 4
        };
        Assert.ThrowsAsync<NotImplementedException>(async () =>
            await this.organizationPackageRepository.Update(newOrganizationPackage));
    }

    #endregion
}