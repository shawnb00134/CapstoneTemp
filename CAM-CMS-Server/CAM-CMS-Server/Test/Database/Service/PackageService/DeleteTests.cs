using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private IPackageService? packageService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var packageRepository = new PackageRepository(context);

        var packageFolderModuleRepository = new PackageFolderModuleRepository(context);

        this.packageService =
            new CAMCMSServer.Database.Service.PackageService(packageRepository, packageFolderModuleRepository);
    }

    [Test]
    public async Task RemoveOneValidPackage()
    {
        var newPackage = new Package { PackageId = 1, IsDeleted = true };
        var countBeforeRemove = MockDataContext.Packages.Count;
        await this.packageService?.Delete(newPackage)!;
        Assert.AreEqual(countBeforeRemove - 1, MockDataContext.Packages.Count);
        Assert.IsFalse(MockDataContext.Packages.Contains(newPackage));
    }

    #endregion
}