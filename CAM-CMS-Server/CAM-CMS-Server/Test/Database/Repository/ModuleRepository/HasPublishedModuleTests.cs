using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class HasPublishedModuleTests
{
    #region Data members

    private IModuleRepository? moduleRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.moduleRepository = new CAMCMSServer.Database.Repository.ModuleRepository(context);
    }

    [Test]
    public async Task HasValidModule()
    {
        var newModule = new Module
        {
            ModuleId = 1
        };
        var result = await this.moduleRepository?.HasPublishedModule(newModule)!;
        Assert.AreEqual(true, result);
    }

    [Test]
    public async Task NoValidModule()
    {
        var newModule = new Module
        {
            ModuleId = 23
        };
        var result = await this.moduleRepository?.HasPublishedModule(newModule)!;
        Assert.AreEqual(false, result);
    }

    [Test]
    public void TestInvalidId()
    {
        var newModule = new Module
        {
            ModuleId = -1
        };
        Assert.ThrowsAsync<ArgumentException>(async () => await this.moduleRepository?.HasPublishedModule(newModule));
    }

    [Test]
    public void TestNullModule()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.moduleRepository?.HasPublishedModule(null!));
    }

    #endregion
}