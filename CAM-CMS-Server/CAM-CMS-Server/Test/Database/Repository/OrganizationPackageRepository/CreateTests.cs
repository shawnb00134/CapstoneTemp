using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationPackageRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private IOrganizationPackageRepository organizationPackageRepository;

    #endregion

  
    #region Methods

    [SetUp]
    public void SetUp() => organizationPackageRepository = new CAMCMSServer.Database.Repository.OrganizationPackageRepository(new MockDataContext());


    [Test]
    public async Task CreateValidOrganizationPackage()
    {
        var newOrganizationPackage = new OrganizationPackage
        {
            OrganizationId = 4,
            PackageId = 4
        };

        await this.organizationPackageRepository.Create(newOrganizationPackage.PackageId,
            newOrganizationPackage.OrganizationId);

        var found = MockDataContext.OrganizationPackages.Where(o =>
            o.OrganizationId == newOrganizationPackage.OrganizationId &&
            o.PackageId == newOrganizationPackage.PackageId);

        Assert.NotNull(found);
        Assert.AreEqual(newOrganizationPackage, found.ElementAt(0));

    }

    [Test]
    public async Task CreateOrganizationPackageInvalidPackageId()
    {
        var newOrganizationPackage = new OrganizationPackage
        {
            OrganizationId = 4,
            PackageId = 0
        };

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.organizationPackageRepository.Create(newOrganizationPackage.PackageId,
                newOrganizationPackage.OrganizationId));
    }
    [Test]
    public async Task CreateOrganizationPackageInvalidOrganizationId()
    {
        var newOrganizationPackage = new OrganizationPackage
        {
            OrganizationId = 0,
            PackageId = 4
        };

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.organizationPackageRepository.Create(newOrganizationPackage.PackageId,
                newOrganizationPackage.OrganizationId));
    }

    [Test]
    public async Task CreateOrganizationPackageNotImplemented()
    {
        var newOrganizationPackage = new OrganizationPackage
        {
            OrganizationId = 0,
            PackageId = 4
        };
        Assert.ThrowsAsync<NotImplementedException>(async () =>
            await this.organizationPackageRepository.Create(newOrganizationPackage));
    }


    #endregion
}

   