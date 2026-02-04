using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

public class OrganizationTests
{
    #region Methods

    [Test]
    public void Equals_ReturnsTrueForEqualObjects()
    {
        // Arrange
        var organization1 = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };

        var organization2 = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };

        // Act
        var result = organization1.Equals(organization2);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentObjects()
    {
        // Arrange
        var organization1 = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };

        var organization2 = new Organization
        {
            OrganizationId = 2,
            Name = "Organization 2",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = false,
            Tags = new[] { "Tag3", "Tag4" }
        };

        // Act
        var result = organization1.Equals(organization2);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentTypes()
    {
        // Arrange
        var organization = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };

        var otherObject = new object();

        // Act
        var result = organization.Equals(otherObject);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var organization1 = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };

        var organization2 = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };

        // Act
        var hashCode1 = organization1.GetHashCode();
        var hashCode2 = organization2.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [Test]
    public void TestGetValues()
    {
        var organization1 = new Organization
        {
            OrganizationId = 1,
            Name = "Organization 1",
            CreatedAt = "2022-01-01",
            UpdatedAt = "2022-02-01",
            IsActive = true,
            Tags = new[] { "Tag1", "Tag2" }
        };
        Assert.AreEqual(organization1.OrganizationId, 1);
        Assert.AreEqual(organization1.Name, "Organization 1");
        Assert.AreEqual(organization1.CreatedAt, "2022-01-01");
        Assert.AreEqual(organization1.UpdatedAt, "2022-02-01");
        Assert.AreEqual(organization1.IsActive, true);
        Assert.AreEqual(organization1.Tags.ElementAt(0), "Tag1");
        Assert.AreEqual(organization1.Tags.ElementAt(1), "Tag2");
    }

    #endregion
}