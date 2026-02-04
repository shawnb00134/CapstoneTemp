using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PackageFolderModuleRepository;

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
        var moduleRepo = new CAMCMSServer.Database.Repository.PackageFolderModuleRepository(this.context);

        Assert.IsNotNull(moduleRepo);
    }

    #endregion
}