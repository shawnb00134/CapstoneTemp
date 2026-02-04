using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class PackageTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var package = new Package { PackageId = 1 };
        var other = new object();

        var result = package.Equals(other);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        // Arrange
        var package = new Package { PackageId = 1 };

        // Act
        var result = package.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSamePackageId_ReturnsTrue()
    {
        // Arrange
        var package = new Package { PackageId = 1 };
        var other = new Package { PackageId = 1 };

        // Act
        var result = package.Equals(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentPackageId_ReturnsFalse()
    {
        var package = new Package { PackageId = 1 };
        var package2 = new Package { PackageId = 2 };

        var result = package.Equals(package2);

        Assert.IsFalse(result);
    }

    [Test]
    public void PackageFolders_DefaultValueNull()
    {
        var package = new Package();

        var folders = package.PackageFolders!;

        Assert.IsNull(folders);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        var package = new Package
        {
            PackageFolders = new List<PackageFolder?>(),
            PackageId = 1,
            CreatedBy = 4,
            CreatedAt = null,
            IsCore = false,
            Name = "Package",
            PackageTypeId = 1,
            PublishedAt = null,
            UpdatedAt = null,
            UpdatedBy = null
        };
        Assert.IsEmpty(package.PackageFolders);
        Assert.AreEqual(1, package.PackageId);
        Assert.AreEqual(package.Name, "Package");
        Assert.AreEqual(package.CreatedBy, 4);
        Assert.AreEqual(package.CreatedAt, null);
        Assert.AreEqual(package.CreatedBy, 4);
        Assert.IsFalse(package.IsCore);
        Assert.AreEqual(package.UpdatedAt, null);
        Assert.AreEqual(package.UpdatedBy, null);
        Assert.AreEqual(package.PackageTypeId, 1);
        Assert.AreEqual(package.PublishedAt, null);
    }

    #endregion
}