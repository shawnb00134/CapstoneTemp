using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PublishedModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetByIdTests
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
    public async Task ValidGetById()
    {
        var module = await this?.repository.GetById(1, 1);

        var expected = MockDataContext.PublishedModules.Where(x => x.Id == 1).ElementAt(0);

        Assert.IsNotNull(module);
        Assert.AreEqual(expected, module);
    }

    [Test]
    public async Task GetAllAtBoundaryTestAsync()
    {
        var module = await this.repository?.GetById(0, 0)!;

        Assert.IsNull(module);
    }

    [Test]
    public async Task AddInvalidModuleIdElement()
    {
        var module = await this.repository?.GetById(-1, 1)!;
        Assert.IsNull(module);
    }

    #endregion
}