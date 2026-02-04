using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class OrganizationPackageTests
{
    #region Methods

    [Test]
    public void Equals_ReturnsTrueForEqualObjects()
    {
        // Arrange
        var organizationPackage1 = new OrganizationPackage { OrganizationId = 1, PackageId = 100 };
        var organizationPackage2 = new OrganizationPackage { OrganizationId = 1, PackageId = 100 };

        // Act
        var result = organizationPackage1.Equals(organizationPackage2);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentObjects()
    {
        // Arrange
        var organizationPackage1 = new OrganizationPackage { OrganizationId = 1, PackageId = 100 };
        var organizationPackage2 = new OrganizationPackage { OrganizationId = 2, PackageId = 200 };

        // Act
        var result = organizationPackage1.Equals(organizationPackage2);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentTypes()
    {
        // Arrange
        var organizationPackage = new OrganizationPackage { OrganizationId = 1, PackageId = 100 };
        var otherObject = new object();

        // Act
        var result = organizationPackage.Equals(otherObject);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var organizationPackage1 = new OrganizationPackage { OrganizationId = 1, PackageId = 100 };
        var organizationPackage2 = new OrganizationPackage { OrganizationId = 1, PackageId = 100 };

        // Act
        var hashCode1 = organizationPackage1.GetHashCode();
        var hashCode2 = organizationPackage2.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    #endregion
}