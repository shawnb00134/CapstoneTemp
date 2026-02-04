using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.LibraryFolderService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task AddValidLibraryFolder()
    {
        var newFolder = new LibraryFolder
        {
            LibraryFolderId = 5,
            Name = "TESTING",
            Description = "TESTING",
            CreatedBy = 4
        };

        var countBeforeAdd = MockDataContext.LibraryFolders.Count;

        await this.service?.Create(newFolder)!;

        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.LibraryFolders.Count);
        Assert.Contains(newFolder, MockDataContext.LibraryFolders);
    }

    [Test]
    public async Task AddInvalidNullLibraryFolder()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.service.Create(null));
    }

    #endregion
}