using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationRespository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DeleteTests
{
    #region Data members

    private IOrganizationRepository organizationRepository;

    #endregion

    #region Methods

    [SetUp]
    public void Setup()
    {
        this.organizationRepository = new OrganizationRepository(new MockDataContext());
    }

    [Test]
    public async Task DeleteOrganizationThrowsNotImplementedException()
    {
        Assert.ThrowsAsync<NotImplementedException>(async () =>
            await this.organizationRepository.Delete(new Organization()));
    }

    #endregion
}