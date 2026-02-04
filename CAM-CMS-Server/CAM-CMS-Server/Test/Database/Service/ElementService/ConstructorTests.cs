using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8618

namespace CAMCMSServer.Test.Database.Service.ElementService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ConstructorTests
{
    #region Data members

    private IElementRepository repo;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repo = new ElementRepository(context);
    }

    [Test]
    public void NotNullTest()
    {
        var elementRepo = new CAMCMSServer.Database.Service.ElementService(this.repo);

        Assert.IsNotNull(elementRepo);
    }

    #endregion
}