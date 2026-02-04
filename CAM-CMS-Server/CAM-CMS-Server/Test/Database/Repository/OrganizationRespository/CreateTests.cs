using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using CAMCMSServer.Test.Database.Context;
using NUnit.Framework;

namespace CAMCMSServer.Test.Database.Repository.OrganizationRespository;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CreateTests
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
    public async Task CreateOneValidOrganization()
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
        await this.organizationRepository.Create(newOrganization);

        var found = MockDataContext.Organizations.Find(o => o.OrganizationId == newOrganization.OrganizationId);

        Assert.AreEqual(newOrganization.OrganizationId, found.OrganizationId);
    }

    [Test]
    public async Task CreateNullOrganization()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.organizationRepository.Create(null));
    }

    #endregion
}