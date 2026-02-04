using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Dapper;

namespace CAMCMSServer.Database.Repository;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<IEnumerable<Organization>> GetAuthorizedOrganizations(int userId);

    Task<bool> DeleteOrganization(int orgId);

    Task<Organization> CreateOrganization(Organization organization, int userId);
}

public class OrganizationRepository : IOrganizationRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public OrganizationRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<Organization>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var organizations = await connection.QueryAsync<Organization>(SqlConstants.SelectAllOrganizations);

        return organizations;
    }

    public async Task<IEnumerable<Organization>> GetAuthorizedOrganizations(int userId)
    {
        using var connection = await this.context.CreateConnection();

        var organizations =
            await connection.QueryAsync<Organization>(SqlConstants.SelectAuthorizedOrganizationsByUserId, new { userId });

        return organizations;
    }

    public async Task<Organization?> GetById(int id, int userId)
    {
        using var connection = await this.context.CreateConnection();

        var result =
            await connection.QueryAsync<Organization>(SqlConstants.SelectOrganizationByOrganizationId, new { id, userId });

        return result.ElementAt(0);
    }

    public async Task<Organization> CreateOrganization(Organization organization, int userId)
    {
        if (organization is null)
        {
            throw new ArgumentNullException("Organization should not be null");
        }

        try
        {

            using var connection = await this.context.CreateConnection();

            await connection.ExecuteAsync(SqlConstants.CreateOrganization, organization);

            var organizationId = await connection.QuerySingleAsync<int>(SqlConstants.GetOrganizationByName,
                new { Name = organization.Name });

            organization.OrganizationId = organizationId;

            var libraryFolder = new LibraryFolder
            {
                Name = organization.Name,
                Description = $"Library folder owned by {organization.Name}",
                CreatedBy = userId
            };

            await connection.ExecuteAsync(SqlConstants.CreateOrgLibraryFolder, libraryFolder);
            var libraryFolderId = await connection.QuerySingleAsync<int>(SqlConstants.GetLibraryFolderByName,
                new { Name = organization.Name });

            var package = new Package
            {
                Name = $"{organization.Name} Package",
                IsCore = false,
                CreatedBy = userId,
                IsDeleted = false
            };

            await connection.ExecuteAsync(SqlConstants.CreateOrgPackage, package);
            var packageId =
                await connection.QuerySingleAsync<int>(SqlConstants.GetPackageByName, new { Name = package.Name });

            var organizationPackage = new
            {
                OrganizationId = organizationId,
                PackageId = packageId,
                IsOwned = true
            };
            await connection.ExecuteAsync(SqlConstants.InsertOrganizationPackage, organizationPackage);

            organization.LibraryFolderId = libraryFolderId;
            await connection.ExecuteAsync(SqlConstants.UpdateOrganizationLibraryFolder,
                new { LibraryFolderId = libraryFolderId, OrganizationId = organizationId });

            var orgContextId =
                await connection.QuerySingleAsync<int>(SqlConstants.GetOrganizationContext, new { organizationId });

            organization.OrganizationId = orgContextId;

            return organization;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while creating the organization", ex);
        }
    }


    public async Task<bool> DeleteOrganization(int organizationId)
    {
        using var connection = await this.context.CreateConnection();
       

        try
        {

            await connection.ExecuteAsync(SqlConstants.DeleteUserAccessRole, new { OrganizationId = organizationId });

            await connection.ExecuteAsync(SqlConstants.DeleteOrgContext, new { OrganizationId = organizationId });

            await connection.ExecuteAsync(SqlConstants.DeleteOrgContentRoleMapper, new { OrganizationId = organizationId });

            await connection.ExecuteAsync(SqlConstants.DeleteOrganizationContentRoles, new { OrganizationId = organizationId });

            await connection.ExecuteAsync(SqlConstants.UnpublishOrganizationPackages, new { OrganizationId = organizationId });

            await connection.ExecuteAsync(SqlConstants.DeleteOrganization, new { OrganizationId = organizationId });

            return true;
        }
        catch
        {
            throw;
        }
    }

    public Task<Organization> Update(Organization updatedElement)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Organization element)
    {
        throw new NotImplementedException();
    }

    public Task<Organization> Create(Organization element)
    {
        throw new NotImplementedException();
    }

    #endregion
}