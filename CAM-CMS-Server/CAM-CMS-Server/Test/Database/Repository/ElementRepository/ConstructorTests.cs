using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Repository.ElementRepository;

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
        var elementRepo = new CAMCMSServer.Database.Repository.ElementRepository(this.context);

        Assert.IsNotNull(elementRepo);
    }

    #endregion
}