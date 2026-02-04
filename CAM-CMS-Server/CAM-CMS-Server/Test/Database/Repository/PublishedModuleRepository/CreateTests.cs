using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PublishedModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
{
    #region Data members

    private IPublishedModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.PublishedModuleRepository(context);
    }

    [Test]
    public async Task AddOneValidPublishedModule()
    {
        var newModule = new PublishedModule
        {
            Id = 4,
            Cache = "Testing"
        };
        var countBeforeAdd = MockDataContext.PublishedModules.Count;

        var result = await this.repository?.Create(newModule)!;

        Assert.NotNull(result);
        Assert.AreEqual(newModule, result);
        Assert.AreEqual(countBeforeAdd + 1, MockDataContext.PublishedModules.Count);
        Assert.Contains(newModule, MockDataContext.PublishedModules);
    }

    [Test]
    public void AddInvalidPublishedModuleId()
    {
        var newModule = new PublishedModule
        {
            Id = -1,
            Cache = "test"
        };
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Create(newModule!)!);
    }

    [Test]
    public void AddNullModule()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Create(null!)!);
    }

    #endregion
}