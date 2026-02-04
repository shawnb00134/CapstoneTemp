using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository;

public interface ILibraryFolderRepository
{
    #region Methods

    Task Delete(LibraryFolder folder);

    Task<IEnumerable<LibraryFolder>> GetAll();

    Task<LibraryFolder> Create(LibraryFolder libraryFolder);

    #endregion

    Task<IEnumerable<LibraryFolder>?> GetAuthorizedLibraryFolders(int userId);
}

public class LibraryFolderRepository : ILibraryFolderRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public LibraryFolderRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task Delete(LibraryFolder folder)
    {
        using var connection = await this.context.CreateConnection();
        idPrecondition(folder);
        var id = folder.LibraryFolderId;
        if (this.containsElements(id).Result || this.containsMethods(id).Result)
        {
            throw new ArgumentOutOfRangeException();
        }

        await connection.ExecuteAsync(SqlConstants.DeleteLibraryFolderById, folder);
    }

    public async Task<IEnumerable<LibraryFolder>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var folders = await connection.QueryAsync<LibraryFolder>(SqlConstants.SelectAllLibraryFolders);

        return folders;
    }

    public async Task<LibraryFolder> Create(LibraryFolder libraryFolder)
    {
        preconditions(libraryFolder);
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertLibraryFolder, libraryFolder);

        return libraryFolder;
    }

    public async Task<IEnumerable<LibraryFolder>?> GetAuthorizedLibraryFolders(int userId)
    {
        using var connection = await this.context.CreateConnection();

        var folders = await connection.QueryAsync<LibraryFolder>(SqlConstants.SelectAuthorizedLibraryFolders, new {userId});
        
        return folders;
    }

    private async Task<bool> containsElements(int id)
    {
        using var connection = await this.context.CreateConnection();

        var elements = await connection.QueryAsync<Element>(SqlConstants.SelectElementsByLibraryId, new { id });

        return elements.Any();
    }

    private async Task<bool> containsMethods(int id)
    {
        using var connection = await this.context.CreateConnection();

        var methods = await connection.QueryAsync<Module>(SqlConstants.SelectAllModulesByLibraryFolderId, new { id });

        return methods.Any();
    }

    private static void idPrecondition(LibraryFolder folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }

        if (folder.LibraryFolderId < 0)
        {
            throw new ArgumentException("Library Folder id out of range { >= 0}");
        }
    }

    private static void preconditions(LibraryFolder folder)
    {
        idPrecondition(folder);

        folder.Description ??= string.Empty;

        if (folder.Name == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }
    }

    #endregion
}