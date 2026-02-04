using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.LibraryFolderService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private ILibraryFolderRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        var context = new MockDataContext();
        this.repository = new LibraryFolderRepository(context);
    }

    [Test]
    public void ValidLibraryFolder()
    {
        var libraryFolderService = new CAMCMSServer.Database.Service.LibraryFolderService(this.repository);

        Assert.NotNull(libraryFolderService);
    }

    #endregion
}