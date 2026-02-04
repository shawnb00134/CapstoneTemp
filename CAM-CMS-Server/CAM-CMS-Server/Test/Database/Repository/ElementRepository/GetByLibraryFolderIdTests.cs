using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByLibraryFolderIdTests
{
    #region Data members

    private IElementRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementRepository(context);
    }

    [Test]
    public async Task GetByValidId()
    {
        var elements = await this.repository?.GetByLibraryFolderId(1)!;

        var expected = MockDataContext.Elements.Where(x => x.LibraryFolderId == 1);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    [Test]
    public async Task GetByInvalidId()
    {
        var elements = await this.repository?.GetByLibraryFolderId(3)!;

        var expected = MockDataContext.Elements.Where(x => x.LibraryFolderId == 3);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    #endregion
}