using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository;

public interface IOrganizationPackageRepository : IRepository<OrganizationPackage>
{
    #region Methods

    public Task<IEnumerable<int>> GetOrganizationIdsByPackageId(int packageId, int userId);

    public Task Create(int packageId, int organizationId);

    public Task Delete(int packageId, int organizationId);

    public Task<IEnumerable<Package>> GetAllOrganizationPackagesByOrganizationId(int organizationId, int userId);

    #endregion
}

public class OrganizationPackageRepository : IOrganizationPackageRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public OrganizationPackageRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public Task<IEnumerable<OrganizationPackage>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<OrganizationPackage?> GetById(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<OrganizationPackage> Create(OrganizationPackage element)
    {
        throw new NotImplementedException();
    }

    public Task<OrganizationPackage> Update(OrganizationPackage updatedElement)
    {
        throw new NotImplementedException();
    }

    public Task Delete(OrganizationPackage element)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<int>> GetOrganizationIdsByPackageId(int packageId, int userId)
    {
        if (packageId < 1) throw new ArgumentException(nameof(packageId) + "must bet > 0");
        using var connection = await context.CreateConnection();

        var organizationIds =
            await connection.QueryAsync<int>(SqlConstants.SelectOrganizationIdsByPackageId, new { packageId, userId });

        return organizationIds;
    }

    public async Task Create(int packageId, int organizationId)
    {
        if (packageId < 1) throw new ArgumentException(nameof(packageId) + "must be > 0");
        if (organizationId < 1) throw new ArgumentException(nameof(organizationId) + "must be > 0");
        using var connection = await context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.CreateOrganizationPackage, new { packageId, organizationId });
    }

    public async Task Delete(int packageId, int organizationId)
    {
        if (packageId < 1) throw new ArgumentException(nameof(packageId) + "must be > 0");
        if (organizationId < 1) throw new ArgumentException(nameof(organizationId) + "must be > 0");
        using var connection = await context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeleteOrganizationPackage, new { packageId, organizationId });
    }

    public async Task<IEnumerable<Package>> GetAllOrganizationPackagesByOrganizationId(int organizationId, int userId)
    {
        if (organizationId < 1) throw new ArgumentException(nameof(organizationId) + "must be > 0");
        using var connection = await context.CreateConnection();

        var organizationPackages =
            await connection.QueryAsync<Package>(SqlConstants.SelectAllPackageByOrganizationId,
                new { organizationId, userId });
        return organizationPackages;
    }

    #endregion
}