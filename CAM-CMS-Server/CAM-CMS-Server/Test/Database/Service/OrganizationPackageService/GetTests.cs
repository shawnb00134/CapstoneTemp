using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationPackageService;

[TestFixture]
[ExcludeFromCodeCoverage]
public class GetTests
{
    #region Data members

    private IOrganizationPackageService organizationPackageService;

    private IPackageService packageService;

    private IPackageRepository packageRepository;

    private IPackageFolderModuleRepository packageFolderModuleRepository;

    private IOrganizationPackageRepository organizationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        var context = new MockDataContext();
        this.organizationRepository = new OrganizationPackageRepository(context);
        this.packageFolderModuleRepository = new PackageFolderModuleRepository(context);
        this.packageRepository = new PackageRepository(context);
        this.packageService = new CAMCMSServer.Database.Service.PackageService(this.packageRepository, this.packageFolderModuleRepository);
        this.organizationPackageService =
            new CAMCMSServer.Database.Service.OrganizationPackageService(this.organizationRepository,
                this.packageService);
    }

    [Test]
    public void GetOrganizationIdsByPackageId()
    {
        var packageId = 1;

        var found = this.organizationPackageService.GetOrganizationIdsByPackageId(packageId, 1);

        var expected = MockDataContext.OrganizationPackages.Where(x => x.PackageId == packageId)
            .Select(x => x.OrganizationId);

        Assert.AreEqual(expected, found.Result);
    }

    [Test]
    public async Task GetAllPackageByOrganizationId()
    {
        var organizationId = 2;

        var found = await this.organizationPackageService.GetAllPackageByOrganizationId(organizationId, 1);

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
    }

    #endregion
}