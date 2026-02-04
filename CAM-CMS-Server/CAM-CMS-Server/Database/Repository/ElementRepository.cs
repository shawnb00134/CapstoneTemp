using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository;

/// <summary>
///     Element repository interface that inherits from the <see cref="IRepository{T}" /> interface.
/// </summary>
/// <seealso cref="webapi.Database.Repository.IRepository&lt;webapi.Model.Element&gt;" />
public interface IElementRepository : IRepository<Element>
{
    #region Methods

    Task<Element> GetLastElement();
    Task<IEnumerable<Element>> GetByLibraryFolderIdDisplayInfo(int id);

    Task<IEnumerable<Element>> GetByLibraryFolderId(int id);

    Task<bool> CheckDelete(Element element);

    Task<Element> UpdateLibraryFolder(Element updatedElement);

    Task<Element> Update(Element updatedElement);

    #endregion
}

/// <summary>
///     Element repository class that implements the <see cref="IElementRepository" /> interface.
///     handles all the element related database operations.
/// </summary>
/// <seealso cref="IElementRepository" />
public class ElementRepository : IElementRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ElementRepository" /> class.</summary>
    /// <param name="context">The context.</param>
    public ElementRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    /// <summary>Gets all the elements.</summary>
    /// <returns>
    ///     all the elements from the db
    /// </returns>
    public async Task<IEnumerable<Element>> GetAll()
    {
        using var connection = await this.context.CreateConnection();

        var elements = await connection.QueryAsync<Element>(SqlConstants.SelectAllElements);

        return elements;
    }

    /// <summary>Gets the element by it's ID.</summary>
    /// <param name="id">The identifier.</param>
    /// <param name="userId"></param>
    /// <returns>
    ///     the element if it exists and null if not found
    /// </returns>
    public async Task<Element?> GetById(int id, int userId)
    {
        idPreconditions(new Element { ElementId = id });

        using var connection = await this.context.CreateConnection();

        var elements = await connection.QueryAsync<Element>(SqlConstants.SelectElementById, new { id });

        try
        {
            return elements.ElementAt(0);
        }
        catch (Exception)
        {
            return null!;
        }
    }

    /// <summary>Gets the element by library folder identifier.</summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    ///     <br />
    /// </returns>
    public async Task<IEnumerable<Element>> GetByLibraryFolderId(int id)
    {
        using var connection = await this.context.CreateConnection();

        var elementRequest =
            await connection.QueryAsync<Element>(SqlConstants.SelectElementsByLibraryId, new { id });

        return elementRequest;
    }

    /// <summary>
    ///     Gets the by library folder identifier display information.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<IEnumerable<Element>> GetByLibraryFolderIdDisplayInfo(int id)
    {
        using var connection = await this.context.CreateConnection();

        var elementRequest =
            await connection.QueryAsync<Element>(SqlConstants.SelectElementsByLibraryIdDisplayInfoOnly, new { id });

        return elementRequest.ToList();
    }

    /// <summary>
    ///     Gets the last element.
    /// </summary>
    /// <returns></returns>
    public async Task<Element> GetLastElement()
    {
        using var connection = await this.context.CreateConnection();

        var elements =
            await connection.QueryAsync<Element>(SqlConstants.SelectAddedElement);

        return elements.ElementAt(0);
    }

    /// <summary>
    ///     Creates the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public async Task<Element> Create(Element element)
    {
        preconditions(element);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertElement, element);

        return element;
    }

    /// <summary>
    ///     Updates the specified updated element.
    /// </summary>
    /// <param name="updatedElement">The updated element.</param>
    /// <returns></returns>
    public async Task<Element> Update(Element updatedElement)
    {
        preconditions(updatedElement);

        using var connection = await this.context.CreateConnection();

        if (updatedElement.TypeId == 7)
        {
            return await this.updateElementTitle(updatedElement);
        }

        await connection.ExecuteAsync(SqlConstants.UpdateElement, updatedElement);

        return updatedElement;
    }

    /// <summary>
    ///     Updates the library folder.
    /// </summary>
    /// <param name="updatedElement">The updated element.</param>
    /// <returns></returns>
    public async Task<Element> UpdateLibraryFolder(Element updatedElement)
    {
        preconditions(updatedElement);
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateElementLibraryFolderId, updatedElement);

        return updatedElement;
    }

    /// <summary>
    ///     Deletes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public async Task Delete(Element element)
    {
        idPreconditions(element);

        await this.deleteLocations(element);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeleteElement, element);
    }

    /// <summary>
    ///     Checks the delete.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public async Task<bool> CheckDelete(Element element)
    {
        using var connection = await this.context.CreateConnection();

        var elements = await connection.QueryAsync<ElementLocation>(SqlConstants.SelectElementLocations, element);

        return elements.Any(); // Conflicts found
    }

    /// <summary>
    ///     Updates the element title.
    /// </summary>
    /// <param name="updatedElement">The updated element.</param>
    /// <returns></returns>
    private async Task<Element> updateElementTitle(Element updatedElement)
    {
        preconditions(updatedElement);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateElementTitle, updatedElement);

        return updatedElement;
    }

    /// <summary>
    ///     Deletes the locations.
    /// </summary>
    /// <param name="element">The element.</param>
    private async Task deleteLocations(Element element)
    {
        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeleteElementLocations, element);
    }

    /// <summary>
    ///     Identifiers the preconditions.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <exception cref="System.ArgumentNullException">element</exception>
    /// <exception cref="System.ArgumentException">Set id out of range. { >= 0 }</exception>
    private static void idPreconditions(Element element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (element.ElementId < 0)
        {
            throw new ArgumentException("Set id out of range. { >= 0 }");
        }
    }

    /// <summary>
    ///     Preconditions the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <exception cref="System.ArgumentNullException">Title</exception>
    /// <exception cref="System.ArgumentException">
    ///     Type id out of range. { > 0 }
    ///     or
    ///     Library folder id out of range.
    /// </exception>
    private static void preconditions(Element element)
    {
        idPreconditions(element);

        if (element.Title == null)
        {
            throw new ArgumentNullException(nameof(element.Title));
        }

        if (element.TypeId <= 0)
        {
            throw new ArgumentException("Type id out of range. { > 0 }");
        }

        if (element.LibraryFolderId <= 0)
        {
            throw new ArgumentException("Library folder id out of range.");
        }

        if (element.CreatedAt != null)
        {
            element.CreatedAt = null;
        }
    }

    #endregion
}