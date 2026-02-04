using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PackageRepositoy;

public class GetAllSubFoldersTests
{
    #region Data members

    private IPackageRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();
        this.repository = new PackageRepository(context);
    }

    [Test]
    public void GetAllSubFoldersValid()
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
        packageFolder.PackageFolders = this.repository.GetAllSubFolders(packageFolder).Result.ToList();

        Assert.AreEqual(1, packageFolder.PackageFolders.Count());
    }

    [Test]
    public void GetAllSubFoldersNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.GetAllSubFolders(null));
    }

    #endregion
}