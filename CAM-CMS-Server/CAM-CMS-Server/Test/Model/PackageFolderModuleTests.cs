using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class PackageFolderModuleTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var package = new PackageFolderModule { PackageFolderModuleId = 1 };
        var other = new object();

        var result = package.Equals(other);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var package = new PackageFolderModule { PackageFolderModuleId = 1 };

        // Act
        var result = package.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSamePackageId_ReturnsTrue()
    {
        // Arrange
        var package = new PackageFolderModule { PackageFolderModuleId = 1 };
        var other = new PackageFolderModule { PackageFolderModuleId = 1 };

        // Act
        var result = package.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentPackageId_ReturnsFalse()
    {
        var package = new PackageFolderModule { PackageFolderModuleId = 1 };
        var package2 = new PackageFolderModule { PackageFolderModuleId = 2 };

        var result = package.Equals(package2);

        Assert.IsFalse(result);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        var packageFolderModule = new PackageFolderModule
        {
            PackageFolderModuleId = 1,
            Cache = "",
            Editable = true,
            OrderInFolder = 0,
            PackageFolderId = 1,
            PublishedModuleId = 1
        };
        Assert.IsTrue(packageFolderModule.Editable);
        Assert.AreEqual(packageFolderModule.PackageFolderId, 1);
        Assert.AreEqual(packageFolderModule.PublishedModuleId, 1);
        Assert.AreEqual(packageFolderModule.PackageFolderModuleId, 1);
        Assert.AreEqual(packageFolderModule.Cache, "");
    }

    #endregion
}