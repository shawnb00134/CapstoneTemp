using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationRespository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetTests
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
    public async Task GetOneValidOrganization()
    {
        var organizationId = 1;

        var organization = await this.organizationRepository.GetById(organizationId, 1);

        Assert.IsNotNull(organization);
        Assert.AreEqual(organizationId, organization.OrganizationId);
    }

    [Test]
    public async Task GetOneInvalidOrganization()
    {
        var organizationId = 0;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await this.organizationRepository.GetById(organizationId, 1));
    }

    [Test]
    public async Task GetAllOrganizations()
    {
        var organizations = await this.organizationRepository.GetAll();

        Assert.IsNotNull(organizations);
        Assert.AreEqual(MockDataContext.Organizations, organizations);
    }

    #endregion
}