using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.PublishedModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
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
    public async Task RemoveOneValidModuleById()
    {
        var newModule = new PublishedModule
        {
            Id = 1
        };
        var countBeforeDelete = MockDataContext.PublishedModules.Count;

        await this.repository?.DeleteById(newModule)!;

        Assert.AreEqual(countBeforeDelete - 1, MockDataContext.PublishedModules.Count);
        Assert.IsFalse(MockDataContext.PublishedModules.Contains(newModule));
    }

    [Test]
    public void DeleteInvalidModuleId()
    {
        var newModule = new PublishedModule
        {
            Id = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.DeleteById(newModule));
    }

    #endregion
}