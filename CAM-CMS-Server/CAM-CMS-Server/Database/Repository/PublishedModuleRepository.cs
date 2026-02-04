using System.Collections.ObjectModel;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository;

public interface IPublishedModuleRepository : IRepository<PublishedModule>
{
    #region Methods

    public Task DeleteById(PublishedModule module);

    #endregion

    Task<int> GetLibraryFolderIdFromModuleId(int publishedModuleId);
}

/// <summary>
///     Repository for published modules.
/// </summary>
/// <seealso cref="IPublishedModuleRepository" />
public class PublishedModuleRepository : IPublishedModuleRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="PublishedModuleRepository" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public PublishedModuleRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Creates the specified module.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <returns></returns>
    public async Task<PublishedModule> Create(PublishedModule module)
    {
        preconditions(module);
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertPublishedModule, module);

        return module;
    }

    /// <summary>
    ///     Deletes the by identifier.
    /// </summary>
    /// <param name="module">The module.</param>
    public async Task DeleteById(PublishedModule module)
    {
        preconditions(module);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeletePublishedModule, module);
    }

    public async Task<int> GetLibraryFolderIdFromModuleId(int publishedModuleId)
    {
        using var connection = await this.context.CreateConnection();

        var result = await connection.QueryAsync<int>(SqlConstants.SelectLibraryFolderIdFromPublishedModuleId,
            new { publishedModuleId });

        return result.FirstOrDefault();
    }

    /// <summary>
    ///     Gets all of the published modules.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<PublishedModule>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var publishedModulesRequests =
            await connection.QueryAsync<PublishedModule>(SqlConstants.SelectAllPublishedModule);

        var publishedModules = new Collection<PublishedModule>();
        foreach (var publishedModule in publishedModulesRequests)
        {
            publishedModules.Add(publishedModule);
        }

        return publishedModules;
    }

    /// <summary>
    ///     Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="userId">The user's id</param>
    /// <returns></returns>
    public async Task<PublishedModule?> GetById(int id, int userId)
    {
        using var connection = await this.context.CreateConnection();

        var publishedModules =
            await connection.QueryAsync<PublishedModule>(SqlConstants.SelectPublishedModuleById, new { id });

        try
        {
            return publishedModules.ElementAt(0);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    ///     Updates the specified module.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <returns></returns>
    public async Task<PublishedModule> Update(PublishedModule module)
    {
        preconditions(module);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdatePublishedModule, module);

        return module;
    }

    /// <summary>
    ///     Deletes the specified element. Not implemented.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    Task IRepository<PublishedModule>.Delete(PublishedModule element)
    {
        throw new NotImplementedException();
    }

    private static void preconditions(PublishedModule module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        if (module.Id < 0)
        {
            throw new ArgumentException("Module id out of range. { >= 0 }");
        }
    }

    #endregion
}