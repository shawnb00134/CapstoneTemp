using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PublishedModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
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
    public async Task ValidModuleUpdate()
    {
        var newModule = new PublishedModule
        {
            Id = 1,
            Cache = "Test: text"
        };
        var result = await this.repository?.Update(newModule)!;

        Assert.IsNotNull(result);
        Assert.AreEqual(newModule, result);

        var actual = MockDataContext.PublishedModules.Where(x => x.Id == newModule.Id).ElementAt(0);

        Assert.AreEqual(newModule.Id, actual.Id);
        Assert.AreEqual(newModule.Cache, actual.Cache);
    }

    [Test]
    public void UpdateNullModule()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Update(null!)!);
    }

    [Test]
    public void InvalidModuleIdUpdate()
    {
        var newModule = new PublishedModule
        {
            Id = -1,
            Cache = "Test: text"
        };
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Update(newModule)!);
    }

    #endregion
}