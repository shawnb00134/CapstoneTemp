using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository;

public interface IPackageFolderModuleRepository : IRepository<PackageFolderModule>
{
    #region Methods

    public Task UpdateOrder(PackageFolderModule packageFolderModule);
    public Task<IEnumerable<PackageFolderModule>> SelectAllFromPackageId(int packageId);

    public Task DeleteById(PackageFolderModule packageFolderModuleId);

    #endregion

    Task<int> GetPackageIdFromFolderId(int modulePackageFolderId);
}

public class PackageFolderModuleRepository : IPackageFolderModuleRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public PackageFolderModuleRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<PackageFolderModule>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<PackageFolderModule>(SqlConstants.SelectAllPackageFolderModules);

        return result;
    }

    public Task<PackageFolderModule?> GetById(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<PackageFolderModule> Create(PackageFolderModule module)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.CreatePackageFolderModule, module);

        return module;
    }

    public async Task<PackageFolderModule> Update(PackageFolderModule folderModule)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdatePackageFolderModule, folderModule);

        return folderModule;
    }

    public Task Delete(PackageFolderModule element)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PackageFolderModule>> SelectAllFromPackageId(int packageId)
    {
        using var connection = await this.context.CreateConnection();

        var result = await
            connection.QueryAsync<PackageFolderModule>(SqlConstants.SelectAllPackageFolderModulesFromPackage,
                new { packageId });

        return result;
    }

    public async Task DeleteById(PackageFolderModule packageFolderModuleId)
    {
        using var connection = await this.context.CreateConnection();

        var result =
            await connection.ExecuteAsync(SqlConstants.DeletePackageFolderModuleById, packageFolderModuleId);
    }

    public async Task<int> GetPackageIdFromFolderId(int modulePackageFolderId)
    {
        using var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<int>(SqlConstants.SelectPackageIdFromFolder,
            new { packageFolderId = modulePackageFolderId });

        return result.FirstOrDefault();
    }

    [ExcludeFromCodeCoverage]
    public async Task UpdateOrder(PackageFolderModule packageFolderModule)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(
            "Call cam_cms.update_package_folder_module(@PackageFolderId,@PackageFolderModuleId,@OrderInFolder)",
            packageFolderModule);
    }

    #endregion
}