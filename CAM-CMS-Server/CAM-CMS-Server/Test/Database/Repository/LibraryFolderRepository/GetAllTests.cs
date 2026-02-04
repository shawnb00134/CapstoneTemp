using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.LibraryFolderRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
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
    public async Task GetAllTestsAsync()
    {
        var folders = await this.repository.GetAll();

        Assert.IsNotNull(folders);
        Assert.AreEqual(MockDataContext.LibraryFolders, folders);
    }

    #endregion
}