using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.PackageService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdatePackageFolderTests
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
    public void UpdateFolderContentValid()
    {
        var packageFolder = new PackageFolder
        {
            ContentRoleId = 1,
            CreatedBy = 2,
            CreatedAt = null,
            DisplayName = "New Name",
            Editable = true,
            FolderTypeId = 1,
            FullDescription = "packageFolder1",
            OrderInParent = 0,
            PackageFolderId = 1,
            PackageFolders = new List<PackageFolder?>(),
            PackageId = 1,
            PackageFoldersModule = new List<PackageFolderModule>(),
            ShortDescription = "package f 1",
            PublishedAt = null,
            Published = false,
            ParentFolderId = null,
            UpdatedAt = null,
            UpdatedBy = null,
            Thumbnail = null
        };
        var before = MockDataContext.PackageFolders
            .FirstOrDefault(x => x.PackageFolderId == packageFolder.PackageFolderId)?.DisplayName;
        this.packageService.UpdateFolderContent(packageFolder);
        var after = MockDataContext.PackageFolders.FirstOrDefault(x =>
            x.PackageFolderId == packageFolder.PackageFolderId);
        Assert.AreNotEqual(before, packageFolder.DisplayName);
        Assert.AreEqual(after.DisplayName, packageFolder.DisplayName);
    }

    [Test]
    public void UpdateFolderContentWithContentIdValid()
    {
        var packageFolder = new PackageFolder
        {
            ContentRoleId = 4,
            CreatedBy = 2,
            CreatedAt = null,
            DisplayName = "New Name",
            Editable = true,
            FolderTypeId = 1,
            FullDescription = "packageFolder1",
            OrderInParent = 0,
            PackageFolderId = 1,
            PackageFolders = new List<PackageFolder?>(),
            PackageId = 1,
            PackageFoldersModule = new List<PackageFolderModule>(),
            ShortDescription = "package f 1",
            PublishedAt = null,
            Published = false,
            ParentFolderId = null,
            UpdatedAt = null,
            UpdatedBy = null,
            Thumbnail = null
        };

        var before = MockDataContext.PackageFolders
            .FirstOrDefault(x => x.PackageFolderId == packageFolder.PackageFolderId).ContentRoleId;
        var subFolderBefore = MockDataContext.PackageFolders
            .FirstOrDefault(x => x.ParentFolderId == packageFolder.PackageFolderId).ContentRoleId;
        this.packageService.UpdateFolderContent(packageFolder);
        var after = MockDataContext.PackageFolders.FirstOrDefault(x =>
            x.PackageFolderId == packageFolder.PackageFolderId);
        var subFolderAfter =
            MockDataContext.PackageFolders.FirstOrDefault(x => x.ParentFolderId == packageFolder.PackageFolderId);

        Assert.AreNotEqual(before, packageFolder.ContentRoleId);
        Assert.AreNotEqual(subFolderBefore, packageFolder.ContentRoleId);
        Assert.AreEqual(after.ContentRoleId, packageFolder.ContentRoleId);
        Assert.AreEqual(subFolderAfter.ContentRoleId, packageFolder.ContentRoleId);
    }

    #endregion
}