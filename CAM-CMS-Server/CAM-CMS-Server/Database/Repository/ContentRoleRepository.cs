using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Dapper;
using Model;

namespace CAMCMSServer.Database.Repository;

public interface IContentRoleRepository
{
    #region Methods

    /// <summary>
    ///     Gets all content roles from the database.
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<ContentRole>> GetAllContentRoles();

    public Task<ContentRole> AddContentRole(ContentRole ContentRole);

    Task<IEnumerable<Archetype>> GetAllArchetypes();

    #endregion
}

public class ContentRoleRepository : IContentRoleRepository
{
    #region Data members

    private readonly IDataContext context;


    #endregion

    #region Constructors

    public ContentRoleRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<ContentRole>> GetAllContentRoles()
    {
        using var connection = await this.context.CreateConnection();

        var contentRoles = await connection.QueryAsync<ContentRole>(SqlConstants.GetAllContentRoles);

        return contentRoles;
    }

    public async Task<ContentRole> AddContentRole(ContentRole contentRole)
    {
        using var connection = await this.context.CreateConnection();
        var sql = SqlConstants.CreateContentRole;

        var parameters = new { Name = contentRole.Name, ArchetypeId = contentRole.ArchetypeId };

        var newContentRole = await connection.QuerySingleAsync<ContentRole>(sql, parameters);

        return newContentRole;
    }

    public async Task<IEnumerable<Archetype>> GetAllArchetypes()
    {
        using var connection = await this.context.CreateConnection();
        return await connection.QueryAsync<Archetype>(SqlConstants.GetAllRoleArchetypes);
    }

    #endregion
}