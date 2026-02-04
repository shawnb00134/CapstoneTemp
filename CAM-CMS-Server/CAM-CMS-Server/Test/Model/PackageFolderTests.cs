using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class PackageFolderTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType()
    {
        var packageFolder = new PackageFolder { PackageFolderId = 1 };
        var other = new object();

        var result = packageFolder.Equals(other);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var package = new PackageFolder { PackageFolderId = 1 };

        // Act
        var result = package.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSamePackageFolderId_ReturnsTrue()
    {
        // Arrange
        var package = new PackageFolder { PackageFolderId = 1 };
        var other = new PackageFolder { PackageFolderId = 1 };

        // Act
        var result = package.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentPackageId_ReturnsFalse()
    {
        var package = new PackageFolder { PackageFolderId = 1 };
        var package2 = new PackageFolder { PackageFolderId = 2 };

        var result = package.Equals(package2);

        Assert.IsFalse(result);
    }

    [Test]
    public void PackageFolders_DefaultValueNull()
    {
        var package = new PackageFolder();

        var folders = package.PackageFolders!;

        Assert.IsNull(folders);
    }

    [Test]
    public void PackageFolderModules_DefaultValueNull()
    {
        var package = new PackageFolder();

        var modules = package.PackageFoldersModule!;

        Assert.IsNull(modules);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        var packageFolders = new PackageFolder
        {
            ContentRoleId = 1,
            PackageFolderId = 1,
            CreatedAt = null,
            CreatedBy = 3,
            DisplayName = "folder",
            Editable = true,
            FolderTypeId = 2,
            FullDescription = "full",
            ShortDescription = "short",
            OrderInParent = 0,
            PublishedAt = null,
            UpdatedAt = null,
            UpdatedBy = null,
            Published = false,
            Thumbnail = null,
            PackageFolders = new List<PackageFolder?>(),
            PackageFoldersModule = new List<PackageFolderModule?>()
        };
        Assert.IsEmpty(packageFolders.PackageFoldersModule);
        Assert.IsEmpty(packageFolders.PackageFolders);
        Assert.AreEqual(packageFolders.CreatedBy, 3);
        Assert.AreEqual(packageFolders.ContentRoleId, 1);
        Assert.AreEqual(packageFolders.CreatedAt, null);
        Assert.AreEqual(packageFolders.DisplayName, "folder");
        Assert.AreEqual(packageFolders.FullDescription, "full");
        Assert.AreEqual(packageFolders.ShortDescription, "short");
        Assert.AreEqual(packageFolders.ContentRoleId, 1);
        Assert.AreEqual(packageFolders.UpdatedAt, null);
        Assert.AreEqual(packageFolders.UpdatedBy, null);
        Assert.AreEqual(packageFolders.Thumbnail, null);
        Assert.AreEqual(packageFolders.OrderInParent, 0);
        Assert.IsTrue(packageFolders.Editable);
        Assert.IsFalse(packageFolders.Published);
    }

    #endregion
}