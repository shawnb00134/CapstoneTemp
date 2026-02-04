using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service
{
    public interface IOrganizationContentRoleService
    {
        public Task CreateOrganizationContentRole(OrganizationContentRole organizationContentRole);

        public Task<IEnumerable<OrganizationContentRole>> GetOrganizationContentRolesForOrganization(int organizationId);

        public Task DeleteOrganizationContentRole(int organizationContentRoleId);

    }
    public class OrganizationContentRoleService : IOrganizationContentRoleService
    {
        #region Data members

        private readonly IOrganizationContentRoleRepository repository;

        #endregion

        #region Constructor

        public OrganizationContentRoleService(IOrganizationContentRoleRepository repository)
        {
            this.repository = repository;
        }

        #endregion

        #region Methods        
        /// <summary>
        ///  Calls the repository to create a new organization content role.
        /// </summary>
        /// <param name="organizationContentRole">The organization content role.</param>
        public async Task CreateOrganizationContentRole(OrganizationContentRole organizationContentRole)
        {
            await this.repository.CreateOrganizationContentRole(organizationContentRole);
        }
        /// <summary>
        /// Calls the repository to get all the OrganizationContentRoles for an organization
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationContentRole>> GetOrganizationContentRolesForOrganization(int organizationId)
        {
            return await this.repository.GetOrganizationContentRolesForOrganization(organizationId);
        }
        /// <summary>
        /// Calls the repository to delete the OrganizationContentRole
        /// </summary>
        /// <param name="organizationContentRoleId">The organization content role identifier.</param>
        public async Task DeleteOrganizationContentRole(int organizationContentRoleId)
        {
            await this.repository.DeleteOrganizationContentRole(organizationContentRoleId);
        }

        #endregion


    }
}
