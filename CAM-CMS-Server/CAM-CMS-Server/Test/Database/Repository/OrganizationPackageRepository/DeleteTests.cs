using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationPackageRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
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
    public async Task DeleteValidOrganizationPackage()
    {
        var organizationPackage = new OrganizationPackage
        {
            OrganizationId = 1,
            PackageId = 1
        };
        var amountBefore = MockDataContext.OrganizationPackages.Count;

        await this.organizationPackageRepository.Delete(organizationPackage.PackageId,
            organizationPackage.OrganizationId);

        var amountAfter = MockDataContext.OrganizationPackages.Count;

        var found = MockDataContext.OrganizationPackages.Where(x =>
            x.PackageId == organizationPackage.PackageId && x.OrganizationId == organizationPackage.OrganizationId);

        Assert.IsEmpty(found);
        Assert.AreNotEqual(amountBefore, amountAfter);
    }

    [Test]
    public async Task DeleteOrganizationPackageInvalidOrganizationId()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await this.organizationPackageRepository.Delete(1, 0));
    }

    [Test]
    public async Task DeleteOrganizationPackageInvalidPackageId()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await this.organizationPackageRepository.Delete(0, 1));
    }

    [Test]
    public async Task DeleteOrganizationPackageNotImplemented()
    {
        var newOrganizationPackage = new OrganizationPackage
        {
            OrganizationId = 0,
            PackageId = 4
        };
        Assert.ThrowsAsync<NotImplementedException>(async () =>
            await this.organizationPackageRepository.Delete(newOrganizationPackage));
    }

    #endregion
}