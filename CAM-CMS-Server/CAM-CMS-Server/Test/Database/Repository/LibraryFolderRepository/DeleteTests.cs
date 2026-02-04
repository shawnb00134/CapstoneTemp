using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.LibraryFolderRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private ILibraryFolderRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.LibraryFolderRepository(context);
    }

    [Test]
    public async Task RemoveOneValidFolder()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = 3,
            Description = "Testing Folder",
            Name = "Name"
        };

        var countBeforeDelete = MockDataContext.LibraryFolders.Count;

        await this.repository?.Delete(folder);

        Assert.AreEqual(countBeforeDelete - 1, MockDataContext.LibraryFolders.Count);
        Assert.IsFalse(MockDataContext.LibraryFolders.Contains(folder));
    }

    [Test]
    public void DeleteNullFolder()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Delete(null!)!);
    }

    [Test]
    public void DeleteInvalidFolderId()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = -1
        };
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Delete(folder));
    }

    [Test]
    public void DeleteInvalidFolderContainingElement()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = 4,
            Description = "Testing Folder 1",
            Name = "Name 1"
        };
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.repository?.Delete(folder));
    }

    [Test]
    public void DeleteInvalidFolderContainingModule()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = 5,
            Description = "Testing Folder 1",
            Name = "Name 1"
        };
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.repository?.Delete(folder));
    }

    [Test]
    public void DeleteInvalidFolderContainingModuleAndElement()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = 1,
            Description = "Testing Folder 1",
            Name = "Name 1"
        };
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.repository?.Delete(folder));
    }

    #endregion
}