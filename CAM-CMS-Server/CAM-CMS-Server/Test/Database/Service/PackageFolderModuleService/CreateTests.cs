using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageFolderModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task AddOneValidPackageFolderModule()
    {
        var newPackageFolderModule = new PackageFolderModule
        {
            PackageFolderModuleId = 1,
            PackageFolderId = 1,
            PublishedModuleId = 1
        };
        var amountBeforeAdd = MockDataContext.PackageFolderModules.Count;

        await this.packageFolderModuleService.Create(newPackageFolderModule);

        Assert.AreEqual(amountBeforeAdd + 1, MockDataContext.PackageFolderModules.Count);
    }

    #endregion
}