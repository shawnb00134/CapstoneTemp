using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PublishedModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IDataContext context;

    #endregion

    #region Methods

    [SetUp]
    public void SetUp()
    {
        this.context = new MockDataContext();
    }

    [Test]
    public void NotNulTest()
    {
        var publishedModuleRepo = new CAMCMSServer.Database.Repository.PublishedModuleRepository(this.context);

        Assert.IsNotNull(publishedModuleRepo);
    }

    #endregion
}