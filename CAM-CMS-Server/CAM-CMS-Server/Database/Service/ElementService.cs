using CAMCMSServer.Database.Repository;
using CAMCMSServer.Model;

namespace CAMCMSServer.Database.Service;

public interface IElementService
{
    #region Methods

    Task<IEnumerable<Element>> GetAll();
    Task<Element> GetById(int id);
    Task<IEnumerable<Element>> GetAllInLibraryFolder(LibraryFolder folder);
    Task<Element> Create(Element model);
    Task<Element> Update(Element updatedElement);
    Task<Element> UpdateLibraryFolder(Element updatedElement);
    Task<bool> Delete(Element element);

    #endregion
}

public class ElementService : IElementService
{
    #region Data members

    private readonly IElementRepository repository;

    #endregion

    #region Constructors

    public ElementService(IElementRepository repository)
    {
        this.repository = repository;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<Element>> GetAll()
    {
        return await this.repository.GetAll();
    }

    public async Task<Element> GetById(int id)
    {
        return await this.repository.GetById(id, 1);
    }

    public async Task<IEnumerable<Element>> GetAllInLibraryFolder(LibraryFolder folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }

        var elements = await this.repository.GetByLibraryFolderIdDisplayInfo(folder.LibraryFolderId);

        return elements;
    }

    public async Task<Element> Create(Element model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        await this.repository.Create(model);
        return await this.repository.GetLastElement();
    }

    public async Task<Element> Update(Element updatedElement)
    {
        if (updatedElement == null)
        {
            throw new ArgumentNullException(nameof(updatedElement));
        }

        await this.repository.Update(updatedElement);
        return await this.repository.GetById((int)updatedElement.ElementId!, 1);
    }

    public async Task<Element> UpdateLibraryFolder(Element updatedElement)
    {
        if (updatedElement == null)
        {
            throw new ArgumentNullException(nameof(updatedElement));
        }

        await this.repository.UpdateLibraryFolder(updatedElement);
        return await this.repository.GetById((int)updatedElement.ElementId!, 1);
    }

    public async Task<bool> Delete(Element element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (await this.repository.CheckDelete(element))
        {
            return false;
        }

        await this.repository.Delete(element);
        return true;
    }

    #endregion
}