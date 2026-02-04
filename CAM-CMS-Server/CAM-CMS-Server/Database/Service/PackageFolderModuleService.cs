using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface IPackageFolderModuleService
{
    #region Methods

    Task Create(PackageFolderModule module);

    Task<IEnumerable<PackageFolderModule>> GetAll();

    Task UpdateOrder(PackageFolderModule module);

    Task Update(PackageFolderModule module);
    Task DeleteById(PackageFolderModule packageFolderModuleId);

    #endregion

    Task<int> GetPackageIdFromFolderId(int modulePackageFolderId);
}

public class PackageFolderModuleService : IPackageFolderModuleService
{
    #region Data members

    private readonly IPackageFolderModuleRepository repository;

    #endregion

    #region Constructors

    public PackageFolderModuleService(IPackageFolderModuleRepository repository)
    {
        this.repository = repository;
    }

    #endregion

    #region Methods

    public async Task Create(PackageFolderModule module)
    {
        await this.repository.Create(module);
    }

    public async Task DeleteById(PackageFolderModule packageFolderModuleId)
    {
        await this.repository.DeleteById(packageFolderModuleId);
    }

    public async Task<int> GetPackageIdFromFolderId(int modulePackageFolderId)
    {
        return await this.repository.GetPackageIdFromFolderId(modulePackageFolderId);
    }

    public async Task<IEnumerable<PackageFolderModule>> GetAll()
    {
        return await this.repository.GetAll();
    }

    public async Task Update(PackageFolderModule module)
    {
        await this.repository.Update(module);
    }

    [ExcludeFromCodeCoverage]
    public async Task UpdateOrder(PackageFolderModule module)
    {
        await this.repository.UpdateOrder(module);
    }

    #endregion
}