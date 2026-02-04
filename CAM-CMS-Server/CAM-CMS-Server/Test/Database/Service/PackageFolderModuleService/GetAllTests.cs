using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageFolderModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
{
    #region Data members

    private IPackageFolderModuleService packageFolderModuleService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var packageFolderModuleRepository = new PackageFolderModuleRepository(context);

        this.packageFolderModuleService =
            new CAMCMSServer.Database.Service.PackageFolderModuleService(packageFolderModuleRepository);
    }

    [Test]
    public async Task TestGetAllPackageFolderModules()
    {
        var modules = await this.packageFolderModuleService.GetAll();

        Assert.IsNotNull(modules);
        Assert.AreEqual(MockDataContext.PackageFolderModules, modules);
    }

    #endregion
}