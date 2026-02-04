using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IPackageRepository packageRepository;

    private IPackageFolderModuleRepository packageFolderModuleRepository;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        var context = new MockDataContext();

        this.packageRepository = new PackageRepository(context);

        this.packageFolderModuleRepository = new PackageFolderModuleRepository(context);
    }

    [Test]
    public void NotNullTest()
    {
        var packageService =
            new CAMCMSServer.Database.Service.PackageService(this.packageRepository, this.packageFolderModuleRepository);

        Assert.IsNotNull(packageService);
    }

    #endregion
}