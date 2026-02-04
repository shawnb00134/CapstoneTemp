using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationRespository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class UpdateTests
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
    public async Task UpdateOrganizationThrowsNotImplementedException()
    {
        //TODO: When method is implemented and used the IDbconnectionWrapper will need to be updated to support UPDATE
        Assert.ThrowsAsync<NotImplementedException>(async () =>
            await this.organizationRepository.Update(new Organization()));
    }

    #endregion
}