using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ContentRoleTests
    {
        [Test]
        public void Equals_WhenOtherIsNull_ReturnsFalse()
        {
            // Arrange
            var contentRole = new ContentRole();
            object other = null;

            // Act
            var result = contentRole.Equals(other);

            // Assert
            Assert.False(result);
        }

        [Test]
        public void Equals_WhenOtherIsNotContentRole_ReturnsFalse()
        {
            // Arrange
            var contentRole = new ContentRole();
            object other = new object();

            // Act
            var result = contentRole.Equals(other);

            // Assert
            Assert.False(result);
        }

        [Test]
        public void Equals_WhenContentRoleIdIsDifferent_ReturnsFalse()
        {
            // Arrange
            var contentRole = new ContentRole { ContentRoleId = 1 };
            var other = new ContentRole { ContentRoleId = 2 };

            // Act
            var result = contentRole.Equals(other);

            // Assert
            Assert.False(result);
        }

        [Test]
        public void Equals_WhenContentRoleIdIsEqual_ReturnsTrue()
        {
            // Arrange
            var contentRole = new ContentRole { ContentRoleId = 1 };
            var other = new ContentRole { ContentRoleId = 1 };

            // Act
            var result = contentRole.Equals(other);

            // Assert
            Assert.True(result);
        }

        [Test]
        public void GetHashCode_ReturnsCorrectHashCode()
        {
            // Arrange
            var contentRole = new ContentRole { ContentRoleId = 1, Name = "Name" };

            // Act
            var result = contentRole.GetHashCode();

            // Assert
            Assert.AreEqual(contentRole.ContentRoleId.GetHashCode() + contentRole.Name.GetHashCode(), result);
        }
    }
}
