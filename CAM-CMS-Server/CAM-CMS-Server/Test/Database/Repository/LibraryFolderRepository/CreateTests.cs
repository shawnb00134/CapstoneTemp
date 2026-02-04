using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618
#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.LibraryFolderRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private ILibraryFolderRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.LibraryFolderRepository(context);
    }

    [Test]
    public async Task AddOneValidFolder()
    {
        var newFolder = new LibraryFolder
        {
            LibraryFolderId = 2,
            Description = "Testing Folder 2",
            Name = "Testing Folder 2"
        };
        var countBeforeAdd = MockDataContext.LibraryFolders.Count;

        var result = await this.repository.Create(newFolder);

        Assert.NotNull(result);
        Assert.AreEqual(newFolder, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.LibraryFolders.Count);
        Assert.Contains(newFolder, MockDataContext.LibraryFolders);
    }

    [Test]
    public async Task AddTwoValidFolders()
    {
        var newFolder = new LibraryFolder
        {
            LibraryFolderId = 1,
            Description = "Testing Folder 1",
            Name = "Testing Folder 1"
        };
        var newFolder2 = new LibraryFolder
        {
            LibraryFolderId = 2,
            Description = "Testing Folder 2",
            Name = "Testing Folder 2"
        };
        var countBeforeAdd = MockDataContext.LibraryFolders.Count;

        var result = await this.repository.Create(newFolder);
        var result2 = await this.repository.Create(newFolder2);

        Assert.NotNull(result);
        Assert.AreEqual(newFolder, result);
        Assert.NotNull(result2);
        Assert.AreEqual(newFolder2, result2);
        Assert.AreEqual(countBeforeAdd + 2, MockDataContext.LibraryFolders.Count);
        Assert.Contains(newFolder, MockDataContext.LibraryFolders);
        Assert.Contains(newFolder2, MockDataContext.LibraryFolders);
    }

    [Test]
    public async Task AddInvalidLibraryFolderDescription()
    {
        var newFolder = new LibraryFolder
        {
            LibraryFolderId = 1,
            Description = null,
            Name = "Testing Folder"
        };
        var countBeforeAdd = MockDataContext.LibraryFolders.Count;

        var result = await this.repository.Create(newFolder);

        Assert.NotNull(result);
        Assert.AreEqual(newFolder, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.LibraryFolders.Count);
        Assert.Contains(newFolder, MockDataContext.LibraryFolders);
    }

    [Test]
    public void AddNullFolder()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.Create(null!));
    }

    [Test]
    public void AddInvalidLibraryFolderId()
    {
        var newFolder = new LibraryFolder
        {
            LibraryFolderId = -1,
            Description = "Testing Folder",
            Name = "Testing Folder"
        };
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository.Create(newFolder));
    }

    [Test]
    public void AddInvalidLibraryFolderName()
    {
        var newFolder = new LibraryFolder
        {
            LibraryFolderId = 1,
            Description = "Testing Folder",
            Name = null
        };
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository.Create(newFolder));
    }

    #endregion
}