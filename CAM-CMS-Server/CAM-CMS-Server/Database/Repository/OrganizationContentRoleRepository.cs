using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository
{
    public interface IOrganizationContentRoleRepository
    {
        public Task CreateOrganizationContentRole(OrganizationContentRole organizationContentRole);

        public Task<IEnumerable<OrganizationContentRole>> GetOrganizationContentRolesForOrganization(int organizationId);

        public Task DeleteOrganizationContentRole(int organizationContentRoleId);
    }
    public class OrganizationContentRoleRepository : IOrganizationContentRoleRepository
    {
        #region Data members

        private readonly IDataContext context;

        #endregion

        #region Constructors

        public OrganizationContentRoleRepository(IDataContext context)
        {
            this.context = context;
        }
        #endregion

        #region Methods     

        /// <summary>
        /// Creates a new organization content role will call the procedure cam_cms.create_organization_content_role to create a new organization content role.
        /// if the organization content role is not created then the database will roll back the transaction.
        /// </summary>
        /// <param name="organizationContentRole">The organization content role.</param>
        public async Task CreateOrganizationContentRole(OrganizationContentRole organizationContentRole)
        {
            try
            {
                using var connection = await this.context.CreateConnection();

                var hasPermission = await connection.QueryAsync<bool?>(SqlConstants.UserCanCreateOrganizationContentRole, organizationContentRole);
                if (!hasPermission.Any())
                {
                    throw new UnauthorizedAccessException("User is not authorized to create organization content role for this organization");
                }

                var parameters = new
                {
                    OrganizationId = organizationContentRole.OrganizationId,
                    DisplayName = organizationContentRole.DisplayName,
                    CreatedBy = organizationContentRole.CreatedBy,
                    ArchetypeIds = organizationContentRole.ArchetypeIds
                };

                await connection.ExecuteAsync(SqlConstants.CreateOrganizationContentRole, parameters);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the organization content role", ex);
            }
        }


        /// <summary>
        /// Gets the organization content roles for organization using the id of the organization
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationContentRole>> GetOrganizationContentRolesForOrganization(int organizationId)
        {
            using var connection = await this.context.CreateConnection();
            try
            {
                var contentRoles = await connection.QueryAsync<OrganizationContentRole>(
                    SqlConstants.SelectOrganizationContentRolesForOrganization, new { organizationId });

                foreach (var role in contentRoles)
                {
                    var rawArchetypeIds = await connection.QueryAsync<dynamic>(
                        SqlConstants.SelectArchetypeIdsForContentRole,
                        new { organizationContentRoleId = role.OrganizationContentRoleId }
                    );

                    var archetypeIds = new List<int>();

                    foreach (var row in rawArchetypeIds)
                    {
                        var idsArray = row.content_role_ids as int[];

                        if (idsArray != null)
                        {
                            archetypeIds.AddRange(idsArray);
                        }
                    }

                    role.ArchetypeIds = archetypeIds;
                }


                return contentRoles;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching the organization content roles", ex);
            }
        }
        /// <summary>
        /// Deletes the organization content role using the id of the organizationContentRole. 
        /// Calls the procedure cam_cms.delete_organization_content_role.
        /// </summary>
        /// <param name="organizationContentRoleId">The organization content role identifier.</param>
        public async Task DeleteOrganizationContentRole(int organizationContentRoleId)
        {
            using var connection = await this.context.CreateConnection();
            // try {
            // var hasPermission = await connection.QueryAsync<bool?>(SqlConstants.UserCanDeleteOrganizationContentRole, new { organizationContentRoleId });
            // }
            // catch (Exception ex)
            // {
            //     throw new ApplicationException("An error occurred while creating the organization content role", ex);
            // }
            // if (!hasPermission.Any())
            // {
            //     throw new UnauthorizedAccessException("User is not authorized to delete organization content role for this organization");
            // }
            try {
            await connection.ExecuteAsync(SqlConstants.DeleteOrganizationContentRole, new { organizationContentRoleId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the organization content role", ex);
            }
        }

        #endregion


    }
}
