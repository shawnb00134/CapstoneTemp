using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.LibraryFolderService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
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
    public async Task GetAllLibraryFolders()
    {
        var folders = await this.service.GetAll();

        Assert.AreEqual(MockDataContext.LibraryFolders, folders);
    }

    #endregion
}