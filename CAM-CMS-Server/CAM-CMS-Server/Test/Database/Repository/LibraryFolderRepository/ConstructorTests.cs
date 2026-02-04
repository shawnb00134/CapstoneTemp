using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Repository.LibraryFolderRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IDataContext context;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();
    }

    [Test]
    public void NotNullTest()
    {
        var folderRepo = new CAMCMSServer.Database.Repository.LibraryFolderRepository(this.context);

        Assert.IsNotNull(folderRepo);
    }

    #endregion
}