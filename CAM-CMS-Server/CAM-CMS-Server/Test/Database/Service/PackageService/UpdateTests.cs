using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageService;

public class UpdateTests
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
    public async Task TestUpdateUnsupported()
    {
        Assert.ThrowsAsync<NotImplementedException>(async () => await this.packageService?.Update(new Package())!);
    }

    #endregion
}