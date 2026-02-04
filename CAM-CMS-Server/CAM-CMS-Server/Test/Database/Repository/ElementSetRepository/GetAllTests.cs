using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ElementSetRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAllTests
{
    #region Data members

    private MockDataContext? context;
    private IElementSetRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void SetupEach()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ElementSetRepository(this.context);
    }

    [Test]
    public async Task GetAllDefaultTest()
    {
        var sets = await this.repository?.GetAll()!;

        Assert.IsNotNull(sets);
        Assert.IsEmpty(sets);
    }

    [Test]
    public async Task GetAllTest()
    {
        var sets = await this.repository?.GetAll(new Module { ModuleId = 1 })!;

        var expected = MockDataContext.Sets.Where(x => x.ModuleId == 1);

        Assert.IsNotNull(sets);
        Assert.AreEqual(expected, sets);
    }

    [Test]
    public async Task GetAllModuleIdAtBoundaryTest()
    {
        var sets = await this.repository?.GetAll(new Module { ModuleId = 0 })!;

        Assert.IsEmpty(sets);
    }

    [Test]
    public void GetAllInvalidModuleTest()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.GetAll(null!)!);
    }

    [Test]
    public void GetAllInvalidModuleIdTest()
    {
        var testModule = new Module
        {
            ModuleId = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.GetAll(testModule));
    }

    #endregion
}