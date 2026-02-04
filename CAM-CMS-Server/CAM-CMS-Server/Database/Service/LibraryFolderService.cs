using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface ILibraryFolderService
{
    #region Methods

    Task<IEnumerable<LibraryFolder>> GetAll();
    Task Create(LibraryFolder folder);
    Task Delete(LibraryFolder element);

    #endregion

    Task<IEnumerable<LibraryFolder>?> GetAuthorizedLibraryFolders(int userId);
}

public class LibraryFolderService : ILibraryFolderService
{
    #region Data members

    private readonly ILibraryFolderRepository repository;

    #endregion

    #region Constructors

    #region Constructor

    public LibraryFolderService(ILibraryFolderRepository repository)
    {
        this.repository = repository;
    }

    #endregion

    #endregion

    #region Methods

    public async Task<IEnumerable<LibraryFolder>> GetAll()
    {
        return await this.repository.GetAll();
    }

    public async Task Create(LibraryFolder folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }

        await this.repository.Create(folder);
    }

    public async Task Delete(LibraryFolder element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        await this.repository.Delete(element);
    }

    public async Task<IEnumerable<LibraryFolder>?> GetAuthorizedLibraryFolders(int userId)
    {
        return await this.repository.GetAuthorizedLibraryFolders(userId);
    }

    #endregion
}