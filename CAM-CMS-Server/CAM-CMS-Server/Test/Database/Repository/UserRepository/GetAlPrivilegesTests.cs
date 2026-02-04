using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.UserRepository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetAlPrivilegesTests
{
    #region Data members

    private IUserRepository repository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.repository = new CAMCMSServer.Database.Repository.UserRepository(context);
    }

    [Test]
    public async Task GetAllPrivileges()
    {
        var contexts = await this.repository.GetAllPrivileges();

        var expected = MockDataContext.Privileges;

        Assert.AreEqual(expected, contexts);
    }

    #endregion
}