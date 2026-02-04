using System.Collections.ObjectModel;
using System.Diagnostics;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Microsoft.IdentityModel.Tokens;

namespace CAMCMSServer.Database.Repository;

public interface IModuleRepository : IRepository<Module>
{
    #region Methods

    Task<IEnumerable<ElementSet>> GetElementSets(int id);
    Task<IEnumerable<ElementLocation>> GetElementLocations(int id);
    Task<IEnumerable<Element>> GetElementsByModule(int id);

    Task<Module> GetByTitle(string? title);
    Task<IEnumerable<Module>> GetByLibraryFolderId(int id);
    Task<bool> HasPublishedModule(Module moduleId);
    Task<Module> UpdateLibraryFolderId(Module module);

    #endregion

    Task<Module> GetModuleFromElementLocation(ElementLocation location);
}

public class ModuleRepository : IModuleRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public ModuleRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<Module>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var modules = await connection.QueryAsync<Module>(SqlConstants.SelectAllModules);

        return modules;
    }

    public async Task<Module?> GetById(int id, int userId)
    {
        idPrecondition(new Module { ModuleId = id });

        using var connection = await this.context.CreateConnection();

        var modules = await connection.QueryAsync<Module>(SqlConstants.SelectModuleById, new { id });

        try
        {
            var module = modules.ElementAt(0);
            module.ElementSets = new Collection<ElementSet>();
            return module;
        }
        catch (Exception)
        {
            throw new ArgumentOutOfRangeException("Module not found");
        }
    }

    public async Task<IEnumerable<ElementSet>> GetElementSets(int id)
    {
        idPrecondition(new Module { ModuleId = id });

        using var connection = await this.context.CreateConnection();

        return await connection.QueryAsync<ElementSet>(SqlConstants.SelectElementSetsByModule, new { ModuleId = id });
    }

    public async Task<IEnumerable<ElementLocation>> GetElementLocations(int id)
    {
        idPrecondition(new Module { ModuleId = id });

        using var connection = await this.context.CreateConnection();

        return await connection.QueryAsync<ElementLocation>(SqlConstants.SelectLocationsByModule,
            new { ModuleId = id });
    }

    public async Task<IEnumerable<Element>> GetElementsByModule(int id)
    {
        idPrecondition(new Module { ModuleId = id });

        using var connection = await this.context.CreateConnection();

        return await connection.QueryAsync<Element>(SqlConstants.SelectElementsByModule, new { ModuleId = id });
    }

    public async Task<Module> GetByTitle(string? title)
    {
        if (title.IsNullOrEmpty())
        {
            throw new ArgumentNullException(title);
        }

        using var connection = await this.context.CreateConnection();

        var module = await connection.QueryAsync<Module>(SqlConstants.SelectModuleByTitle, new { title });

        return module.ElementAt(0);
    }

    public async Task<IEnumerable<Module>> GetByLibraryFolderId(int id)
    {
        using var connection = await this.context.CreateConnection();

        var modules = await connection.QueryAsync<Module>(SqlConstants.SelectAllModulesByLibraryFolderId, new { id });

        return modules;
    }

    public async Task<Module> Create(Module module)
    {
        preconditions(module);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertModule, module);

        return module;
    }

    public async Task<Module> UpdateLibraryFolderId(Module module)
    {
        preconditions(module);

        var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateModuleLibraryId, module);

        return module;
    }

    public async Task<Module> GetModuleFromElementLocation(ElementLocation location)
    {
        using var connection = await this.context.CreateConnection();

        var modules = await connection.QueryAsync<Module>(SqlConstants.SelectModuleByElementLocation, location);

        return modules.ElementAt(0);
    }

    public async Task<bool> HasPublishedModule(Module module)
    {
        idPrecondition(module);

        var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<PublishedModule>(SqlConstants.HasPublishedModule, module);

        return result.ToList().Count >= 1;
    }

    public async Task<Module> Update(Module updatedModule)
    {
        Debug.WriteLine(updatedModule);
        preconditions(updatedModule);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateModule, updatedModule);

        return updatedModule;
    }

    public async Task Delete(Module module)
    {
        idPrecondition(module);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeleteModule, module);
    }

    private static void idPrecondition(Module module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        if (module.ModuleId < 0)
        {
            throw new ArgumentException("Module id out of range. { >= 0 }");
        }
    }

    private static void preconditions(Module module)
    {
        idPrecondition(module);

        if (module.Title == null)
        {
            throw new ArgumentNullException(nameof(module.Title));
        }

        if (module.DisplayTitle == null)
        {
            throw new ArgumentNullException(nameof(module.Title));
        }

        if (module.IsTemplate == null)
        {
            throw new ArgumentNullException(nameof(module.IsTemplate));
        }

        if (module.LibraryFolderId <= 0)
        {
            throw new ArgumentException("Library folder id out of range.  { > 0 }");
        }

        if (module.CreatedAt != null)
        {
            module.CreatedAt = null;
        }
    }

    #endregion
}