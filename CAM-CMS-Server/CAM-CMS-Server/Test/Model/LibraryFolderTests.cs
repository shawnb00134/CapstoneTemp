using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using NUnit.Framework;

namespace CAMCMSServer.Test.Model;

[ExcludeFromCodeCoverage]
[TestFixture]
public class LibraryFolderTests
{
    #region Methods

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var folder = new LibraryFolder { LibraryFolderId = 1 };
        var other = new object();

        var result = folder.Equals(other);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithNullObject_ReturnsFalse()
    {
        var folder = new LibraryFolder { LibraryFolderId = 1 };

        var result = folder.Equals(null);

        Assert.IsFalse(result);
    }

    [Test]
    public void Equals_WithSameLibraryId_ReturnsTrue()
    {
        var folder = new LibraryFolder { LibraryFolderId = 1 };
        var folder2 = new LibraryFolder { LibraryFolderId = 1 };

        var result = folder.Equals(folder2);

        Assert.IsTrue(result);
    }

    [Test]
    public void Equals_WithDifferentLibraryId_ReturnsFalse()
    {
        var folder = new LibraryFolder { LibraryFolderId = 1 };
        var folder2 = new LibraryFolder { LibraryFolderId = 2 };

        var result = folder.Equals(folder2);

        Assert.IsFalse(result);
    }

    [Test]
    public void Test_SetPropertyValues()
    {
        var folder = new LibraryFolder
        {
            Modules = new List<Module>(),
            Elements = new List<Element>(),
            LibraryFolderId = 2,
            Description = "Testing Description",
            Name = "name",
            CreatedBy = 4
        };

        Assert.AreEqual(2, folder.LibraryFolderId);
        Assert.AreEqual("Testing Description", folder.Description);
        Assert.AreEqual("name", folder.Name);
        Assert.AreEqual(4, folder.CreatedBy);
        Assert.IsEmpty(folder.Modules);
        Assert.IsEmpty(folder.Elements);
        Assert.NotNull(folder.GetHashCode());
    }

    #endregion
}