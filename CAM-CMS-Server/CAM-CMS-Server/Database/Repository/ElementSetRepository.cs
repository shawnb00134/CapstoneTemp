using System.Data;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;
using Dapper;
using Newtonsoft.Json.Linq;

namespace CAMCMSServer.Database.Repository;

public interface IElementSetRepository : IRepository<ElementSet>
{
    #region Methods

    Task<IEnumerable<ElementSet>> GetAll(Module module);

    Task<IEnumerable<ElementLocation>> GetSetElements(ElementSet set);

    Task CreateLocation(ElementLocation location);

    Task UpdateLocation(ElementLocation location);

    Task UpdateLocationElement(ElementLocation updatedLocation);

    Task UpdateSetOrder(ElementSet updatedSet);

    Task UpdateLocationOrder(ElementLocation updatedLocation);

    Task UpdateLocationSet(ElementLocation updatedLocationSet, int newSetId);

    Task UpdateElementLocationAttributes(ElementLocation location);

    Task DeleteLocation(ElementLocation location);

    #endregion

    Task<int> GetLibraryFolderIdFromModuleId(int? newSetModuleId);
    Task<int> GetParentModuleLibraryFolderIdFromElementLocation(ElementLocation newElement);
    Task<int> GetLibraryFolderIdFromElementSet(ElementSet elementSet);
}

public class ElementSetRepository : IElementSetRepository
{
    #region Data members

    private readonly IDataContext context;

    #endregion

    #region Constructors

    public ElementSetRepository(IDataContext context)
    {
        this.context = context;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<ElementSet>> GetAll()
    {
        return await this.GetAll(new Module { ModuleId = 0 });
    }

    public async Task<ElementSet?> GetById(int id, int userId)
    {
        setIdPreconditions(new ElementSet { SetLocationId = id });

        using var connection = await this.context.CreateConnection();

        var sets = await connection.QueryAsync<ElementSet>(SqlConstants.SelectElementSetById, new { id });

        try
        {
            return sets.ElementAt(0);
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<IEnumerable<ElementSet>> GetAll(Module module)
    {
        if (module == null)
        {
            throw new ArgumentNullException(nameof(module));
        }

        if (module.ModuleId < 0)
        {
            throw new ArgumentException("Module id out of range. { > 0 }");
        }

        using var connection = await this.context.CreateConnection();

        var sets = await connection.QueryAsync<ElementSet>(SqlConstants.SelectElementSetsByModule, module);

        return sets;
    }

    public async Task<IEnumerable<ElementLocation>> GetSetElements(ElementSet set)
    {
        setIdPreconditions(set);

        using var connection = await this.context.CreateConnection();

        var locations = await connection.QueryAsync<ElementLocation>(SqlConstants.SelectElementLocationsBySet, set);

        return locations;
    }

    public async Task<ElementSet> Create(ElementSet element)
    {
        setPreconditions(element);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertElementSet, element);

        return element;
    }

    public async Task CreateLocation(ElementLocation location)
    {
        locationPreconditions(location);

        if (location.Element is { TypeId: 7 })
        {
            var contentObject = JObject.Parse(location.Element.Content);
            location.Attributes.HeadingLevel = contentObject["headingLevel"].Value<int>();
        }

        if (location.Element is { TypeId: 2 or 5 })
        {
            location.Attributes.DisplayLink = false;
        }

        location.Attributes.Width = "500";
        location.Attributes.Height = "auto";
        location.Attributes.Alignment = "left";

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.InsertElementLocation, location);
    }

    public async Task<ElementSet> Update(ElementSet updatedElement)
    {
        setPreconditions(updatedElement);

        await this.updateElements(updatedElement); // TODO: Move this?

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateElementSet, updatedElement);

        return updatedElement;
    }

    public async Task UpdateSetOrder(ElementSet updatedSet)
    {
        setPreconditions(updatedSet);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync("CALL cam_cms.update_element_set(@SetLocationId, @Place)", updatedSet);
    }

    public async Task UpdateLocation(ElementLocation location)
    {
        locationPreconditions(location);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateElementLocation, location);
    }

    public async Task UpdateLocationElement(ElementLocation updatedLocation)
    {
        locationPreconditions(updatedLocation);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.UpdateLocationElement, updatedLocation);
    }

    public async Task UpdateLocationOrder(ElementLocation updatedLocation)
    {
        locationPreconditions(updatedLocation);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync("CALL cam_cms.update_set_location(@SetLocationId, @ElementId, @Place)",
            updatedLocation);
    }

    public async Task UpdateLocationSet(ElementLocation updatedLocationSet, int newSetId)
    {
        locationPreconditions(updatedLocationSet);

        using var connection = await this.context.CreateConnection();

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@CurrentId", updatedLocationSet.SetLocationId, DbType.Int16);
        dynamicParameters.Add("@Place", updatedLocationSet.Place, DbType.Int16);
        dynamicParameters.Add("@NewId", newSetId, DbType.Int16);
        dynamicParameters.Add("@ElementId", updatedLocationSet.ElementId, DbType.Int16);

        await connection.ExecuteAsync(SqlConstants.UpdateLocationSet, dynamicParameters);

        dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@CurrentId", updatedLocationSet.SetLocationId, DbType.Int16);

        await connection.ExecuteAsync("CALL cam_cms.fix_set_ordering(@CurrentId)",
            dynamicParameters); // Fix ordering of set the location was in
    }

    public async Task UpdateElementLocationAttributes(ElementLocation location)
    {
        locationPreconditions(location);
        using var connection = await this.context.CreateConnection();
        await connection.ExecuteAsync(SqlConstants.UpdateElementLocationAttributes, location);
    }

    public async Task Delete(ElementSet set)
    {
        setIdPreconditions(set);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync("CALL cam_cms.delete_element_set(@SetLocationId)", set);
    }

    public async Task DeleteLocation(ElementLocation location)
    {
        locationIdPreconditions(location);

        using var connection = await this.context.CreateConnection();

        await connection.ExecuteAsync(SqlConstants.DeleteElementLocation, location);
    }

    public async Task<int> GetLibraryFolderIdFromModuleId(int? newSetModuleId)
    {
        using var connection = await this.context.CreateConnection();

        var folderId = await connection.QueryAsync<int>(SqlConstants.SelectLibraryFolderIdFromPublishedModuleId, new { publishedModuleId = newSetModuleId });

        return folderId.FirstOrDefault();
    }

    public async Task<int> GetParentModuleLibraryFolderIdFromElementLocation(ElementLocation newElement)
    {
        using var connection = await this.context.CreateConnection();

        var folderId = await connection.QueryAsync<int>(SqlConstants.SelectParentModuleLibraryFolderIdFromElementLocation, newElement);

        return folderId.FirstOrDefault();
    }

    public async Task<int> GetLibraryFolderIdFromElementSet(ElementSet elementSet)
    {
        using var connection = await this.context.CreateConnection();

        var folderId = await connection.QueryAsync<int>(SqlConstants.SelectLibraryFolderIdFromElementSet, elementSet);

        return folderId.FirstOrDefault();
    }

    private async Task updateElements(ElementSet updatedElement)
    {
        if (updatedElement.Elements == null)
        {
            return;
        }

        foreach (var element in updatedElement.Elements)
        {
            if (element == null)
            {
                continue;
            }

            await this.UpdateLocation(element); // TODO: Move this?
        }
    }

    private static void setIdPreconditions(ElementSet element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (element.SetLocationId < 0)
        {
            throw new ArgumentException("Set id out of range. { >= 0 }");
        }
    }

    private static void setPreconditions(ElementSet element)
    {
        setIdPreconditions(element);

        if (element.ModuleId <= 0)
        {
            throw new ArgumentException("Module id out of range. { > 0 }");
        }

        if (element.Place < 0)
        {
            throw new ArgumentException("Place in module out of range. { >= 0 }");
        }

        if (element.IsEditable == null)
        {
            throw new ArgumentNullException(nameof(element.IsEditable));
        }
    }

    private static void locationIdPreconditions(ElementLocation element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (element.SetLocationId <= 0)
        {
            throw new ArgumentException("Set id out of range. { > 0 }");
        }

        if (element.ElementId <= 0)
        {
            throw new ArgumentException("Element id out of range. { > 0 }");
        }
    }

    private static void locationPreconditions(ElementLocation element)
    {
        locationIdPreconditions(element);

        if (element.Place < 0)
        {
            throw new ArgumentException("Place in module out of range. { >= 0 }");
        }

        if (element.IsEditable == null)
        {
            throw new ArgumentNullException(nameof(element.IsEditable));
        }
    }

    #endregion
}