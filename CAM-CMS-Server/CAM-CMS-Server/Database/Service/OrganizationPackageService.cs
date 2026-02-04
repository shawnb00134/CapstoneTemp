using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface IOrganizationPackageService
{
    #region Methods

    Task<IEnumerable<int>> GetOrganizationIdsByPackageId(int packageId, int userId);

    Task UpdateAssociatedOrganizations(PackageAndOrganizations packageAndOrganizations, int userId);

    Task UpdateAssociatedPackages(OrganizationAndPackages organizationAndPackages, int userId);

    Task<IEnumerable<Package>> GetAllPackageByOrganizationId(int organizationId, int userId);

    #endregion
}

public class OrganizationPackageService : IOrganizationPackageService
{
    #region Data members

    private readonly IOrganizationPackageRepository repository;
    private readonly IPackageService packageService;

    #endregion

    #region Constructors

    public OrganizationPackageService(IOrganizationPackageRepository repository, IPackageService packageService)
    {
        this.repository = repository;
        this.packageService = packageService;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<int>> GetOrganizationIdsByPackageId(int packageId, int userId)
    {
        var organizationIds = await repository.GetOrganizationIdsByPackageId(packageId, userId);

        return organizationIds;
    }

    public async Task UpdateAssociatedOrganizations(PackageAndOrganizations packageAndOrganizations, int userId)
    {
        var oldOrganizationIds = await repository.GetOrganizationIdsByPackageId(packageAndOrganizations.PackageId, userId);

        var oldOrganizationIdsMissing = oldOrganizationIds.Except(packageAndOrganizations.OrganizationIds);

        var newOrganizationIds = packageAndOrganizations.OrganizationIds.Except(oldOrganizationIds);

        var creationThread = new Thread(() =>
        {
            foreach (var organizationId in newOrganizationIds)
            {
                this.repository.Create(packageAndOrganizations.PackageId, organizationId);
            }
        });
        var deletionThread = new Thread(() =>
        {
            foreach (var organizationId in oldOrganizationIdsMissing)
            {
                this.repository.Delete(packageAndOrganizations.PackageId, organizationId);
            }
        });
        creationThread.Start();
        deletionThread.Start();

        creationThread.Join();
        deletionThread.Join();
    }

    public async Task UpdateAssociatedPackages(OrganizationAndPackages organizationAndPackages, int userId)
    {
        var oldPackageIds =
            (await repository.GetAllOrganizationPackagesByOrganizationId(organizationAndPackages.OrganizationId, userId))
            .Select(p => p.PackageId);


        var packageIdsMissing = oldPackageIds.Except(organizationAndPackages.PackageIds);

        var newPackageIds = organizationAndPackages.PackageIds.Except(packageIdsMissing);

        var creationThread = new Thread(() =>
        {
            foreach (var packageId in newPackageIds)
                repository.Create(packageId, organizationAndPackages.OrganizationId);
        });
        var deletionThread = new Thread(() =>
        {
            foreach (var packageId in packageIdsMissing)
                repository.Delete(packageId, organizationAndPackages.OrganizationId);
        });
        creationThread.Start();
        deletionThread.Start();

        creationThread.Join();
        deletionThread.Join();
    }


    public async Task<IEnumerable<Package>> GetAllPackageByOrganizationId(int organizationId, int userId)
    {
        var organizationPackages = await repository.GetAllOrganizationPackagesByOrganizationId(organizationId, userId);

        var packageArray = organizationPackages.ToArray();

        var tasks = packageArray.Select(async package => await packageService.GetById(package.PackageId, userId));

        packageArray = await Task.WhenAll(tasks);


        return packageArray;
    }

    #endregion
}