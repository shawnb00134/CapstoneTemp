using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationPackageService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
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
    }

    [Test]
    public void NotNullTest()
    {
        var orgPackage =
            new CAMCMSServer.Database.Service.OrganizationPackageService(this.organizationRepository,
                this.packageService);

        Assert.IsNotNull(orgPackage);
    }

    #endregion
}