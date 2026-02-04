using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateLibraryFolderTests
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
    public async Task UpdateValidElement()
    {
        var element = MockDataContext.Elements.Find(x => x.ElementId == 1);
        element.LibraryFolderId = 5;

        var actual = await this.elementService.UpdateLibraryFolder(element);

        Assert.AreEqual(actual, element);
        Assert.AreEqual(actual, MockDataContext.Elements.Find(x => x.ElementId == 1));
    }

    [Test]
    public async Task UpdateNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.elementService?.UpdateLibraryFolder(null));
    }

    #endregion
}