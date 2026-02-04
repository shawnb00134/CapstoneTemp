using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

#pragma warning disable CS8602

namespace CAMCMSServer.Test.Database.Repository.ModuleRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private IModuleRepository? repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.ModuleRepository(context);
    }

    [Test]
    public async Task RemoveOneValidElement()
    {
        var newModule = new Module { ModuleId = 1, CreatedAt = "delete" };

        var countBeforeDelete = MockDataContext.Modules.Count;

        await this.repository?.Delete(newModule)!;

        Assert.AreEqual(countBeforeDelete - 1, MockDataContext.Modules.Count);
        Assert.IsFalse(MockDataContext.Modules.Contains(newModule));
    }

    [Test]
    public void DeleteNullElement()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.repository?.Delete(null!)!);
    }

    [Test]
    public void DeleteInvalidModuleIdElement()
    {
        var newModule = new Module
        {
            ModuleId = -1
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await this.repository?.Delete(newModule));
    }

    #endregion
}