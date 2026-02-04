using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Service.OrganizationService;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task CreateValidOrganization()
    {
        var newOrganization = new Organization
        {
            Name = "Test Organization",
            IsActive = true,
            Tags = new[] { "Test", "Organization" },
            CreatedAt = null,
            OrganizationId = 1,
            UpdatedAt = null
        };
        await this.organizationService.Create(newOrganization, 1);

        var found = MockDataContext.Organizations.Find(o => o.OrganizationId == newOrganization.OrganizationId);

        Assert.AreEqual(newOrganization.OrganizationId, found.OrganizationId);
    }

    [Test]
    public async Task CreateNullOrganization()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.organizationRepository.Create(null));
    }

    #endregion

    #region Data Members

    private IOrganizationRepository organizationRepository;

    private IOrganizationService organizationService;

    #endregion
}