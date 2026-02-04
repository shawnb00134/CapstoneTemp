using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task AddOneValidPackage()
    {
        var newPackage = new Package
        {
            CreatedAt = null,
            CreatedBy = 2,
            IsCore = true,
            Name = "Package 1",
            PackageFolders = new List<PackageFolder>(),
            PackageId = 1,
            PackageTypeId = 1,
            PublishedAt = null,
            UpdatedAt = null,
            UpdatedBy = null
        };
        var countBeforeAdd = MockDataContext.Packages.Count;

        await this.packageService?.Create(newPackage)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.Packages.Count);
        Assert.Contains(newPackage, MockDataContext.Packages);
    }

    #endregion
}