using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace CAMCMSServer.Database.Repository;

public interface IPackageRepository : IRepository<Package>
{
    #region Methods

    Task<Package> GetByName(string? name);

    Task<IEnumerable<PackageFolder>> GetFolders(Package package);

    Task<PackageFolder> CreateFolder(PackageFolder package);

    Task UpdateFolder(PackageFolder package);

    Task UpdateFolderContentWithContentID(PackageFolder package);

    Task UpdateFolderContent(PackageFolder package);
    Task<int?> GetPackageFolderContentIdByFolderId(int folderId);

    Task DeleteFolder(PackageFolder package);

    Task ReorderFolder(PackageFolder package);

    Task<IEnumerable<PackageFolder>> GetAllSubFolders(PackageFolder inFolder);

    Task<IEnumerable<PackageFolder>> GetSubFolders(PackageFolder packageFolder);

    Task<IEnumerable<PackageFolderModule>> GetAllSubFolderModules(PackageFolder packageFolder);

    #endregion

    Task<IEnumerable<Package>?> GetAuthorizedPackages(int userId);
    Task<int> GetPackageIdFromFolder(PackageFolder packageFolder);
}

public class PackageRepository : IPackageRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ElementRepository" /> class.</summary>
    /// <param name="context">The context.</param>
    public PackageRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<Package>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var packages = await connection.QueryAsync<Package>(SqlConstants.SelectAllPackages);

        return packages;
    }

    public async Task<Package?> GetById(int id, int userId)
    {
        using var connection = await this.context.CreateConnection();

        var packages = await connection.QueryAsync<Package>(SqlConstants.SelectPackageById, new { id, userId });

        try
        {
            return packages.ElementAt(0);
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<Package> GetByName(string? name)
    {
        if (name.IsNullOrEmpty())
        {
            throw new ArgumentNullException(name);
        }

        using var connection = await this.context.CreateConnection();

        var packages = await connection.QueryAsync<Package>(SqlConstants.SelectPackageByName, new { name });

        try
        {
            return packages.ElementAt(0);
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<IEnumerable<PackageFolder>> GetFolders(Package package)
    {
        using var connection = await this.context.CreateConnection();

        var folders =
            await connection.QueryAsync<PackageFolder>(SqlConstants.SelectAllPackageFoldersForPackage, package);

        return folders;
    }

    public async Task<Package> Create(Package package)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertPackage, package);

        return package;
    }

    public async Task<PackageFolder> CreateFolder(PackageFolder folder)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertPackageFolder, folder);

        return folder;
    }

    public async Task<Package> Update(Package updatedElement)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Package package)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeletePackage, package);
    }

    public async Task DeleteFolder(PackageFolder package)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeletePackageFolder, package);
    }

    /// <summary>
    ///     Updates the packageFolder
    ///     The method appends the folder to the end of it's parent folder and fixes the ordering of the folder that was moved
    ///     from
    /// </summary>
    /// <param name="package">The package.</param>
    public async Task UpdateFolder(PackageFolder package)
    {
        Log.Information("In UpdateFolder in class PackageRepository");
        Log.Information(package.ToString());

        using var connection = this.context.CreateConnection().Result;
        var oldParentResult = await connection.QueryAsync<PackageFolder>(SqlConstants.SelectPackageFolderById, package);
        var oldParentId = oldParentResult.ToList().ElementAt(0);
        Log.Information("Old Parent " + oldParentId);
        if (package.ParentFolderId == null)
        {
            Log.Information("Parent Folder Id is null");
            var result =
                await connection.QueryAsync<PackageFolder>(SqlConstants.SelectPackageFoldersInTopLevel, package);
            var foldersCount = result.ToList().Count;
            package.OrderInParent = foldersCount;
            await connection.ExecuteAsync(SqlConstants.UpdatePackageFolder, package);
        }
        else
        {
            Log.Information("Updating folder and reordering in folder: " + package.ParentFolderId);
            var result =
                await connection.QueryAsync<PackageFolder>(SqlConstants.SelectPackageFoldersInParentFolder, package);
            var foldersCount = result.ToList().Count;
            package.OrderInParent = foldersCount;
            await connection.ExecuteAsync(SqlConstants.UpdatePackageFolder, package);
        }

        if (oldParentId.ParentFolderId != null)
        {
            Log.Information("Fixing ordering in old parent folder: " + oldParentId.ParentFolderId);
            await connection.ExecuteAsync("call cam_cms.fix_package_folder_ordering(@ParentFolderId)", oldParentId);
        }
        else
        {
            Log.Information("Fixing ordering in the root level of folders");
            await connection.ExecuteAsync("call cam_cms.fix_root_folder_ordering(@PackageId)", oldParentId);
        }
    }

    [ExcludeFromCodeCoverage]
    public async Task ReorderFolder(PackageFolder package)
    {
        using var connection = this.context.CreateConnection().Result;

        await connection.ExecuteAsync("Call cam_cms.update_package_folder(@PackageFolderId,@OrderInParent)",
            package);
    }

    /// <summary>
    ///     Updates the PackageFolder content
    /// </summary>
    /// <precondition>package != null</precondition>
    /// <param name="package"></param>
    public async Task UpdateFolderContent(PackageFolder package)
    {
        if (package == null)
        {
            throw new ArgumentNullException(nameof(package));
        }

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdatePackageFolder, package);
    }

    /// <summary>
    ///     Updates the folder and then calls the a procedure to update all the child folders content role id to match the new
    ///     content role id.
    /// </summary>
    /// <precondition>package != null</precondition>
    /// <precondition>package != null</precondition>
    /// <param name="package">The package folder.</param>
    public async Task UpdateFolderContentWithContentID(PackageFolder package)
    {
        if (package == null)
        {
            throw new ArgumentNullException(nameof(package));
        }

        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.UpdatePackageFolder, package);
        await connection.ExecuteAsync(SqlConstants.UpdateAllChildFoldersContentRoleId, package);
    }

    /// <summary>
    ///     Gets the package folder content identifier by folder identifier.
    /// </summary>
    /// <param name="folderId">The folder identifier.</param>
    /// <returns>The folders content role id</returns>
    public async Task<int?> GetPackageFolderContentIdByFolderId(int folderId)
    {
        using var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<int?>(SqlConstants.SelectContentRoleIdByFolderId, new { folderId });
        if (result.Count() == 0)
        {
            return null;
        }

        return result.ElementAt(0);
    }

    /// <summary>
    ///     Gets all the sub folders for a package folder.
    /// </summary>
    /// <precondition>parentFolder != null</precondition>
    /// <param name="parentFolder">The parent parentFolder</param>
    /// <returns>all the subfolders for the parentFolder</returns>
    public async Task<IEnumerable<PackageFolder>> GetAllSubFolders(PackageFolder parentFolder)
    {
        if (parentFolder == null)
        {
            throw new ArgumentNullException(nameof(parentFolder));
        }

        using var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<PackageFolder>(SqlConstants.SelectAllChildFolders, parentFolder);

        return result;
    }

    public async Task<IEnumerable<PackageFolder>> GetSubFolders(PackageFolder packageFolder)
    {
        if (packageFolder == null)
        {
            throw new ArgumentNullException(nameof(packageFolder));
        }

        using var connection = await this.context.CreateConnection();
        var result = await connection.QueryAsync<PackageFolder>(SqlConstants.SelectAllChildFolders, packageFolder);

        return result;
    }

    public async Task<IEnumerable<PackageFolderModule>> GetAllSubFolderModules(PackageFolder packageFolder)
    {
        if (packageFolder == null)
        {
            throw new ArgumentNullException(nameof(packageFolder));
        }

        using var connection = await this.context.CreateConnection();

        var result =
            await connection.QueryAsync<PackageFolderModule>(SqlConstants.SelectAllPackageFolderModulesForPackageFolder,
                packageFolder);

        return result;
    }

    public async Task<IEnumerable<Package>?> GetAuthorizedPackages(int userId)
    {
        using var connection = await this.context.CreateConnection();

        var result = 
            await connection.QueryAsync<Package>(SqlConstants.SelectAuthorizedPackages, new { userId });

        return result;
    }

    public async Task<int> GetPackageIdFromFolder(PackageFolder packageFolder)
    {
        using var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<int>(SqlConstants.SelectPackageIdFromFolder, packageFolder);

        return result.ElementAt(0);
    }

    #endregion
}