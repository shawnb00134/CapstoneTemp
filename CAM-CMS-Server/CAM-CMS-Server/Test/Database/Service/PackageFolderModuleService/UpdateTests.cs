using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageFolderModuleService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
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
    public async Task UpdateOneValidPackageFolderModuleAsync()
    {
        var newPackage = new PackageFolderModule
        {
            PackageFolderId = 1,
            Cache = "",
            Editable = true,
            OrderInFolder = 3,
            PackageFolderModuleId = 1,
            PublishedModuleId = 1
        };
        var beforeUpdate = this.packageFolderModuleService.GetAll().Result.ToList()
            .Where(x => x.PackageFolderModuleId == newPackage.PackageFolderModuleId).ToList().ElementAt(0);
        var orderBefore = beforeUpdate.OrderInFolder;
        await this.packageFolderModuleService.Update(newPackage);
        var afterUpdate = this.packageFolderModuleService.GetAll().Result.ToList()
            .Where(x => x.PackageFolderModuleId == newPackage.PackageFolderModuleId).ToList().ElementAt(0);

        Assert.AreNotEqual(newPackage.OrderInFolder, orderBefore);
        Assert.AreEqual(newPackage.OrderInFolder, afterUpdate.OrderInFolder);
    }

    #endregion
}