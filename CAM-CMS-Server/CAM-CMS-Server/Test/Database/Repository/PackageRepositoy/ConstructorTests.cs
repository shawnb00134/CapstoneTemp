using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PackageRepositoy;

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
        var packageRepo = new PackageRepository(this.context);

        Assert.IsNotNull(packageRepo);
    }

    #endregion
}