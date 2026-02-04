using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetTests
{
    #region Methods

    [SetUp]
    public void Setup()
    {
        var context = new MockDataContext();

        this.organizationRepository = new OrganizationRepository(context);
        this.organizationService = new CAMCMSServer.Database.Service.OrganizationService(this.organizationRepository);
    }

    [Test]
    public async Task GetOneValidOrganization()
    {
        var organizationId = 1;

        var organization = await this.organizationService.GetById(organizationId, 1);

        Assert.IsNotNull(organization);
        Assert.AreEqual(organizationId, organization.OrganizationId);
    }

    [Test]
    public async Task GetOneInvalidOrganization()
    {
        var organizationId = 0;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await this.organizationService.GetById(organizationId, 1));
    }

    [Test]
    public async Task GetAllOrganizations()
    {
        var organizations = await this.organizationService.GetAll();

        Assert.IsNotNull(organizations);
        Assert.AreEqual(MockDataContext.Organizations, organizations);
    }

    #endregion

    #region Data Members

    private IOrganizationRepository organizationRepository;

    private IOrganizationService organizationService;

    #endregion
}