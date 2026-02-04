using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class OrganizationContentRoleTests
    {
        #region Methods

        [Test]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            // Arrange
            var role = new OrganizationContentRole { OrganizationContentRoleId = 1 };
            var other = new object();

            // Act
            var result = role.Equals(other);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_WithNullObject_ReturnsFalse()
        {
            // Arrange
            var role = new OrganizationContentRole { OrganizationContentRoleId = 1 };

            // Act
            var result = role.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_WithSameOrganizationContentRoleId_ReturnsTrue()
        {
            // Arrange
            var role = new OrganizationContentRole { OrganizationContentRoleId = 1 };
            var other = new OrganizationContentRole { OrganizationContentRoleId = 1 };

            // Act
            var result = role.Equals(other);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_WithDifferentOrganizationContentRoleId_ReturnsFalse()
        {
            // Arrange
            var role = new OrganizationContentRole { OrganizationContentRoleId = 1 };
            var other = new OrganizationContentRole { OrganizationContentRoleId = 2 };

            // Act
            var result = role.Equals(other);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DisplayName_DefaultValueIsNull()
        {
            // Arrange
            var role = new OrganizationContentRole();

            // Act
            var displayName = role.DisplayName;

            // Assert
            Assert.IsNull(displayName);
        }

        [Test]
        public void CreatedAt_DefaultValueIsNull()
        {
            // Arrange
            var role = new OrganizationContentRole();

            // Act
            var createdAt = role.CreatedAt;

            // Assert
            Assert.IsNull(createdAt);
        }

        [Test]
        public void UpdatedAt_DefaultValueIsNull()
        {
            // Arrange
            var role = new OrganizationContentRole();

            // Act
            var updatedAt = role.UpdatedAt;

            // Assert
            Assert.IsNull(updatedAt);
        }

        [Test]
        public void Test_SetPropertyValues()
        {
            // Arrange & Act
            var role = new OrganizationContentRole
            {
                OrganizationContentRoleId = 1,
                OrganizationId = 2,
                ContentRoleId = 3,
                DisplayName = "Testing Role",
                CreatedAt = "CREATED",
                CreatedBy = 4,
                UpdatedAt = "UPDATED",
                UpdatedBy = 5
            };

            // Assert
            Assert.AreEqual(1, role.OrganizationContentRoleId);
            Assert.AreEqual(2, role.OrganizationId);
            Assert.AreEqual(3, role.ContentRoleId);
            Assert.AreEqual("Testing Role", role.DisplayName);
            Assert.AreEqual("CREATED", role.CreatedAt);
            Assert.AreEqual(4, role.CreatedBy);
            Assert.AreEqual("UPDATED", role.UpdatedAt);
            Assert.AreEqual(5, role.UpdatedBy);
            Assert.NotNull(role.GetHashCode());
        }

        #endregion
    }
}
