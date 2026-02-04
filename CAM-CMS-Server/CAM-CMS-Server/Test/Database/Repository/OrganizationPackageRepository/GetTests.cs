using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationPackageRepository;

[TestFixture]
[ExcludeFromCodeCoverage]
public class GetTests
{
    #region Data members

    private IOrganizationPackageRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        this.repository =
            new CAMCMSServer.Database.Repository.OrganizationPackageRepository(new MockDataContext());
    }

    [Test]
    public void GetPackagesGetAllNotImplemented()
    {
        Assert.Throws<NotImplementedException>(() => this.repository.GetAll());
    }

    [Test]
    public void GetPackagesGetByIdNotImplemented()
    {
        Assert.Throws<NotImplementedException>(() => this.repository.GetById(1, 1));
    }

    #endregion

    #region GetOrganizationIdsByPackageId

    [Test]
    public async Task GetOrganizationIdsByPackageId()
    {
        var packageId = 1;

        var found = await this.repository.GetOrganizationIdsByPackageId(packageId, 1);

        var expected = MockDataContext.OrganizationPackages.Where(x => x.PackageId == packageId)
            .Select(x => x.OrganizationId);

        Assert.NotNull(found);
        Assert.AreEqual(expected, found);
    }

    [Test]
    public async Task GetOrganizationIdsByPackageIdNotFound()
    {
        var packageId = 100;

        var found = await this.repository.GetOrganizationIdsByPackageId(packageId, 1);

        var expected = MockDataContext.OrganizationPackages.Where(x => x.PackageId == packageId)
            .Select(x => x.OrganizationId);

        Assert.NotNull(found);
        Assert.AreEqual(expected, found);
    }

    [Test]
    public async Task GetOrganizationIdsInvalidPackageId()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository.GetOrganizationIdsByPackageId(0, 1));
    }

    #endregion

    #region GetPackagesByOrganizaitonId

    [Test]
    public async Task GetPackagesByOrganizationIdSingle()
    {
        var organizationId = 1;

        var found = await this.repository.GetAllOrganizationPackagesByOrganizationId(organizationId, 1);

        var orgPackageId = MockDataContext.OrganizationPackages.Where(x => x.OrganizationId == organizationId).ToList()
            .ElementAt(0);
        var expected = MockDataContext.Packages.Where(x => x.PackageId == orgPackageId.PackageId);

        Assert.NotNull(found);
        Assert.AreEqual(expected, found.ToList());
    }

    [Test]
    public async Task GetPackagesByOrganizationIdMulti()
    {
        var organizationId = 1;

        var found = await this.repository.GetAllOrganizationPackagesByOrganizationId(organizationId, 1);

        var orgPackageId = MockDataContext.OrganizationPackages.Where(x => x.OrganizationId == organizationId).ToList();
        var expected = new List<Package>();
        foreach (var package in MockDataContext.Packages)
        {
            foreach (var id in orgPackageId)
            {
                if (package.PackageId == id.PackageId)
                {
                    expected.Add(package);
                    break;
                }
            }
        }

        Assert.NotNull(found);
        Assert.AreEqual(expected, found.ToList());
    }

    [Test]
    public async Task GetPackagesByOrganizationIdNoneFound()
    {
        var organizationId = 900;

        Assert.IsEmpty(this.repository.GetAllOrganizationPackagesByOrganizationId(organizationId, 1).Result);
    }

    [Test]
    public async Task GetPackagesByOrganizationIdInvalidOrgId()
    {
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.repository.GetAllOrganizationPackagesByOrganizationId(0, 1));
    }

    #endregion
}