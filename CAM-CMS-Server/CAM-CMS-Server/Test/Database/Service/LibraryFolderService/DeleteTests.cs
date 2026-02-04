using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.LibraryFolderService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private ILibraryFolderService service;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        var context = new MockDataContext();
        var repository = new LibraryFolderRepository(context);

        this.service = new CAMCMSServer.Database.Service.LibraryFolderService(repository);
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

        await this.service?.Delete(folder);

        Assert.AreEqual(countBeforeDelete - 1, MockDataContext.LibraryFolders.Count);
        Assert.IsFalse(MockDataContext.LibraryFolders.Contains(folder));
    }

    [Test]
    public void DeleteNullFolder()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.service?.Delete(null!)!);
    }

    [Test]
    public void DeleteInvalidFolderId()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = -1
        };
        Assert.ThrowsAsync<ArgumentException>(async () => await this.service?.Delete(folder));
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
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service?.Delete(folder));
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
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service?.Delete(folder));
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
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.service?.Delete(folder));
    }

    #endregion
}