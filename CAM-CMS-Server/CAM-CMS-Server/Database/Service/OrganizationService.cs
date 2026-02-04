using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface IOrganizationService
{
    #region Methods

    Task Create(Organization organization, int userId);

    Task<IEnumerable<Organization>> GetAll();

    Task<Organization> GetById(int id, int userId);

    Task<bool> DeleteOrganization(int organizationId);

    #endregion

    Task<IEnumerable<Organization>> GetAuthorizedOrganizations(int userId);
}

public class OrganizationService : IOrganizationService
{
    #region Data members

    private readonly IOrganizationRepository repository;

    #endregion

    #region Constructors

    public OrganizationService(IOrganizationRepository repository)
    {
        this.repository = repository;
    }

    #endregion

    #region Methods

    public async Task Create(Organization organization, int userId)
    {
        await this.repository.CreateOrganization(organization, userId);
    }

    public Task<IEnumerable<Organization>> GetAll()
    {
        var organizations = this.repository.GetAll();

        return organizations;
    }

    public async Task<IEnumerable<Organization>> GetAuthorizedOrganizations(int userId)
    {
        var organizations = await this.repository.GetAuthorizedOrganizations(userId);

        return organizations;
    }

    public async Task<Organization> GetById(int id, int userId)
    {
        var organization = await this.repository.GetById(id, userId);

        return organization;
    }

    public async Task<bool> DeleteOrganization(int organizationId)
    {
        await repository.DeleteOrganization(organizationId);
        return true;
    }

    #endregion
}