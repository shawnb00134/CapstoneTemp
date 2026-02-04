using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByIdTests
{
    #region Data members

    private MockDataContext? context;
    private IModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ModuleRepository(this.context);
    }

    [Test]
    public async Task GetAllTestAsync()
    {
        var module = await this.repository?.GetById(1, 1)!;

        var expected = MockDataContext.Modules.Where(x => x.ModuleId == 1).ElementAt(0);

        Assert.IsNotNull(module);
        Assert.AreEqual(expected, module);
    }

    [Test]
    public async Task GetAllAtBoundaryTestAsync()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await this.repository?.GetById(0, 0)!; });
    }

    [Test]
    public async Task GetAllAboveBoundaryTestAsync()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await this.repository?.GetById(100, 1)!; });
    }

    [Test]
    public void AddInvalidModuleIdElement()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.GetById(-1, 1));
    }

    #endregion
}