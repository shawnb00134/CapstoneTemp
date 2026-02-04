using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;
using Model;

namespace CAMCMSServer.Database.Service;

public interface IContentRoleService
{
    #region Methods

    Task<IEnumerable<ContentRole>> GetAllContentRoles();

    Task<ContentRole> AddContentRole(ContentRole contentRole);

    Task<IEnumerable<Archetype>> GetAllArchetypes();

    #endregion
}

public class ContentRoleService : IContentRoleService
{
    #region Data members

    #region Data member

    private readonly IContentRoleRepository repository;

    #endregion

    #endregion

    #region Constructors

    #region Constructor

    public ContentRoleService(IContentRoleRepository repository)
    {
        this.repository = repository;
    }

    #endregion

    #endregion

    #region Methods

    public async Task<IEnumerable<ContentRole>> GetAllContentRoles()
    {
        var contentRoles = await this.repository.GetAllContentRoles();

        return contentRoles;
    }

    public async Task<ContentRole> AddContentRole(ContentRole contentRole)
    {
        return await this.repository.AddContentRole(contentRole);
    }

    public async Task<IEnumerable<Archetype>> GetAllArchetypes()
    {
        return await this.repository.GetAllArchetypes();
    }


    #endregion
}