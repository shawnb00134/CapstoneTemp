using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllInLibraryFolderTests
{
    #region Data members

    private IElementService? elementService;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        var elementRepository = new ElementRepository(context);
        this.elementService = new CAMCMSServer.Database.Service.ElementService(elementRepository);
    }

    [Test]
    public async Task GetByValidId()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = 1
        };

        var elements = await this.elementService?.GetAllInLibraryFolder(folder)!;

        var expected = MockDataContext.Elements.Where(x => x.LibraryFolderId == folder.LibraryFolderId);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    [Test]
    public async Task GetByInvalidId()
    {
        var folder = new LibraryFolder
        {
            LibraryFolderId = 3
        };

        var elements = await this.elementService?.GetAllInLibraryFolder(folder)!;

        var expected = MockDataContext.Elements.Where(x => x.LibraryFolderId == folder.LibraryFolderId);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    [Test]
    public async Task GetByNullLibrary()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementService?.GetAllInLibraryFolder(null));
    }

    #endregion
}