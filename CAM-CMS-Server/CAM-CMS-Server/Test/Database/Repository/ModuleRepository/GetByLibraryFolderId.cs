using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByLibraryFolderId
{
    #region Data members

    private IModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ModuleRepository(context);
    }

    [Test]
    public async Task GetByValidId()
    {
        var elements = await this.repository?.GetByLibraryFolderId(1)!;

        var expected = MockDataContext.Modules.Where(x => x.LibraryFolderId == 1);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    [Test]
    public async Task GetByInvalidId()
    {
        var elements = await this.repository?.GetByLibraryFolderId(0)!;

        var expected = MockDataContext.Modules.Where(x => x.LibraryFolderId == 3);

        Assert.IsNotNull(elements);
        Assert.AreEqual(expected, elements);
    }

    #endregion
}