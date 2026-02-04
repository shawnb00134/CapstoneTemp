using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface IPackageService
{
    #region Methods

    Task<IEnumerable<Package>> GetAll();

    Task<Package?> GetById(int packageId, int userId);

    Task<Package> Create(Package package);

    Task<Package?> CreateFolder(PackageFolder folder);

    Task<Package?> Update(Package package);

    Task UpdateFolder(PackageFolder package);

    Task ReorderFolder(PackageFolder package);

    Task<PackageFolder> UpdateFolderContent(PackageFolder packageFolder);

    Task Delete(Package package);

    Task DeleteFolder(PackageFolder package);

    Task<IEnumerable<PackageFolder>> GetSubFolders(PackageFolder packageFolder);

    Task<IEnumerable<PackageFolderModule>> GetAllSubFolderModules(PackageFolder packageFolder);

    #endregion

    Task<IEnumerable<Package>?> GetAuthorizedPackages(int userId);

    Task<int> GetPackageIdFromFolder(PackageFolder packageFolder);
}

public class PackageService : IPackageService
{
    #region Data members

    private readonly IPackageRepository repository;

    private readonly IPackageFolderModuleRepository folderModuleRepository;
    private IPackageService packageServiceImplementation;

    #endregion

    #region Constructors

    public PackageService(IPackageRepository repository, IPackageFolderModuleRepository folderModuleRepository)
    {
        this.repository = repository;
        this.folderModuleRepository = folderModuleRepository;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<Package>> GetAll()
    {
        return await this.repository.GetAll();
    }

    [ExcludeFromCodeCoverage]
    public async Task<Package?> GetById(int packageId, int userId)
    {
        var package = await this.repository.GetById(packageId, userId);

        if (package == null)
        {
            return null;
        }

        var enumerableFolders = await this.repository.GetFolders(package);
        var subModules = await this.folderModuleRepository.SelectAllFromPackageId(package.PackageId);
        var folders = enumerableFolders.ToList();
        var modules = subModules.ToList();



        package.PackageFolders = folders.Where(f => f.ParentFolderId == null);
        return package;
    }

    public async Task<Package> Create(Package package)
    {
        await this.repository.Create(package);

        return await this.repository.GetByName(package.Name);
    }

    public async Task<Package?> CreateFolder(PackageFolder folder)
    {
        if (folder?.PackageId == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }

        await this.repository.CreateFolder(folder);

        return await this.GetById((int)folder.PackageId, (int)folder.CreatedBy);
    }

    public async Task<Package?> Update(Package package)
    {
        await this.repository.Update(package);

        return await this.repository.GetById(package.PackageId, (int)package.UpdatedBy!);
    }

    public async Task Delete(Package package)
    {
        await this.repository.Delete(package);
    }

    public async Task UpdateFolder(PackageFolder package)
    {
        await this.repository.UpdateFolder(package);
    }

    public async Task DeleteFolder(PackageFolder package)
    {
        await this.repository.DeleteFolder(package);
    }

    /// <summary>
    ///     Updates the content of the folder. If the content role is changed, it will update the content role and get all the
    ///     sub folders for the folder.
    /// </summary>
    /// <param name="packageFolder">The packageFolder.</param>
    /// <returns>The updated packageFolder Folder</returns>
    public async Task<PackageFolder> UpdateFolderContent(PackageFolder packageFolder)
    {
        var oldContentRole = await this.repository.GetPackageFolderContentIdByFolderId(packageFolder.PackageFolderId);
        if (oldContentRole != packageFolder.ContentRoleId)
        {
            await this.repository.UpdateFolderContentWithContentID(packageFolder);
            packageFolder.PackageFolders = await this.GetAllSubFolders(packageFolder);
        }
        else
        {
            await this.repository.UpdateFolderContent(packageFolder);
        }

        return packageFolder;

    }

    [ExcludeFromCodeCoverage]
    public async Task ReorderFolder(PackageFolder package)
    {
        await this.repository.ReorderFolder(package);
    }

    /// <summary>
    ///     Gets all the sub folders for a parent folder and its children.
    ///     This method recursively calls itself to get all the sub folders and has the base case when there are no more sub
    ///     folders it returns;
    /// </summary>
    /// <param name="inFolder">The parent folder</param>
    /// <returns>selectedFolders which is all the subfolders for the inFolder</returns>
    public async Task<IEnumerable<PackageFolder>> GetAllSubFolders(PackageFolder inFolder)
    {
        var selectedFolders = await this.repository.GetAllSubFolders(inFolder);
        if (selectedFolders.Count() == 0)
        {
            return selectedFolders;
        }

        foreach (var folder in selectedFolders.ToList())
        {
            folder.PackageFolders = await this.GetAllSubFolders(folder);
        }

        return selectedFolders;
    }

    public async Task<IEnumerable<PackageFolder>> GetSubFolders(PackageFolder packageFolder)
    {
        return await this.repository.GetSubFolders(packageFolder);
    }

    public async Task<IEnumerable<PackageFolderModule>> GetAllSubFolderModules(PackageFolder packageFolder)
    {
        return await this.repository.GetAllSubFolderModules(packageFolder);
    }

    public Task<IEnumerable<Package>?> GetAuthorizedPackages(int userId)
    {
        return this.repository.GetAuthorizedPackages(userId);
    }

    public Task<int> GetPackageIdFromFolder(PackageFolder packageFolder)
    {
        return this.repository.GetPackageIdFromFolder(packageFolder);
    }

    #endregion
}