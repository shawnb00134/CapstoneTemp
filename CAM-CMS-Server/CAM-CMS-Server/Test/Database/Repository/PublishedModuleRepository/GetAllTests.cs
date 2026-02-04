using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PublishedModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
{
    #region Data members

    private MockDataContext? context;
    private IPublishedModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.PublishedModuleRepository(this.context);
    }

    [Test]
    public async Task GetAllTestAsync()
    {
        var modules = await this.repository?.GetAll()!;

        Assert.IsNotNull(modules);
        Assert.AreEqual(MockDataContext.PublishedModules, modules);
    }

    #endregion
}