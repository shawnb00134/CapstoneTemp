using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationPackageService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateAndDeleteTests
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
    public async Task CreateOrganizationPackagesOnly()
    {
        var packageAndOrganizations = new PackageAndOrganizations()
        {
            OrganizationIds = new List<int>()
            {
                4, 5
            },
            PackageId = 2
                
        };
        var amountBefore = MockDataContext.OrganizationPackages.Count(x => x.PackageId == packageAndOrganizations.PackageId);

        await this.organizationPackageService.UpdateAssociatedOrganizations(packageAndOrganizations, 1);

        var found = MockDataContext.OrganizationPackages.Where(
            x => x.PackageId == packageAndOrganizations.PackageId);

        var amountAfter =
            MockDataContext.OrganizationPackages.Count(x => x.PackageId == packageAndOrganizations.PackageId);

        Assert.IsNotEmpty(found.ToList());
        Assert.AreNotEqual(amountBefore,amountAfter);
    }
    [Test]
    public async Task DeleteOrganizationPackagesOnly()
    {
        var packageAndOrganizations = new PackageAndOrganizations()
        {
            OrganizationIds = new List<int>(),
            PackageId = 1

        };
        var amountBefore = MockDataContext.OrganizationPackages.Count(x => x.PackageId == packageAndOrganizations.PackageId);

        await this.organizationPackageService.UpdateAssociatedOrganizations(packageAndOrganizations, 1);

        var found = MockDataContext.OrganizationPackages.Where(
            x => x.PackageId == packageAndOrganizations.PackageId);

        var amountAfter =
            MockDataContext.OrganizationPackages.Count(x => x.PackageId == packageAndOrganizations.PackageId);

        Assert.IsEmpty(found.ToList());
        Assert.AreNotEqual(amountBefore, amountAfter);
    }

    [Test]
    public async Task AddAndDeleteOrganizationPackages()
    {
        var packageAndOrganizations = new PackageAndOrganizations()
        {
            OrganizationIds = new List<int>()
            {
                1,6
            },
            PackageId = 1

        };

        var packagesBefore =
            MockDataContext.OrganizationPackages.Where(x => x.PackageId == packageAndOrganizations.PackageId);

        var index0 = packagesBefore.ElementAt(0);
        var index1 = packagesBefore.ElementAt(1);

        await this.organizationPackageService.UpdateAssociatedOrganizations(packageAndOrganizations, 1);

        var found = MockDataContext.OrganizationPackages.Where(
            x => x.PackageId == packageAndOrganizations.PackageId);

        var index0After = packagesBefore.ElementAt(0);
        var index1After = packagesBefore.ElementAt(1);

        Assert.IsNotEmpty(found);
        Assert.AreEqual(index0, index0After);
        Assert.AreNotEqual(index1,index1After);

    }

    [Test]
    public async Task CreatePackagesInOrganizationOnly()
    {
        var organizationPackages = new OrganizationAndPackages()
        {
            OrganizationId = 1,
            PackageIds = new List<int>()
            {
                1, 3
            }
        };
        var amountBefore = MockDataContext.OrganizationPackages.Count(x => x.OrganizationId == organizationPackages.OrganizationId);

        await this.organizationPackageService.UpdateAssociatedPackages(organizationPackages, 1);

        var found = MockDataContext.OrganizationPackages.Where(x =>
            x.OrganizationId == organizationPackages.OrganizationId);
        var amountAfter =
            MockDataContext.OrganizationPackages.Count(x =>
                x.OrganizationId == organizationPackages.OrganizationId);
        Assert.IsNotEmpty(found.ToList());
        Assert.AreNotEqual(amountAfter, amountBefore);
    }

    [Test]
    public async Task DeletePackagesInOrganizationOnly()
    {

        var organizationPackages = new OrganizationAndPackages()
        {
            OrganizationId = 1,
            PackageIds = new List<int>()
        };

        var amountBefore = MockDataContext.OrganizationPackages.Count(x => x.OrganizationId == organizationPackages.OrganizationId);

        await this.organizationPackageService.UpdateAssociatedPackages(organizationPackages, 1);

        var found = MockDataContext.OrganizationPackages.Where(x =>
            x.OrganizationId == organizationPackages.OrganizationId);
        var amountAfter =
            MockDataContext.OrganizationPackages.Count(x =>
                x.OrganizationId == organizationPackages.OrganizationId);
        Assert.IsNotEmpty(found.ToList());
        Assert.AreNotEqual(amountAfter, amountBefore);
    }

    [Test]
    public async Task CreateAndDeletePackagesInOrganization()
    {
        var organizationPackages = new OrganizationAndPackages()
        {
            OrganizationId = 1,
            PackageIds = new List<int>()
            {
                1,6
            }
        };
        var foundBefore = MockDataContext.OrganizationPackages.Where(x => x.OrganizationId == organizationPackages.OrganizationId);
        var index0 = foundBefore.ElementAt(0);
        var index1 = foundBefore.ElementAt(1);

        await this.organizationPackageService.UpdateAssociatedPackages(organizationPackages, 1);
        var foundAfter= MockDataContext.OrganizationPackages.Where(x => x.OrganizationId == organizationPackages.OrganizationId);
        var index0After = foundAfter.ElementAt(0);
        var index1After = foundAfter.ElementAt(1);
        Assert.IsNotEmpty(foundAfter);
        Assert.AreEqual(index0, index0After);
        Assert.AreEqual(index1, index1After);

    }

    #endregion
    

    


}