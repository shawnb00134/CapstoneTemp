using CAMCMSServer.Database.Mappers;
using CAMCMSServer.Model.ElementTypes;
using Newtonsoft.Json;

namespace CAMCMSServer.Model;

public class Element
{
    #region Properties

    [JsonProperty("confirmedDelete")]public bool? ConfirmedDelete { get; set; }

    [Column("element_id")][JsonProperty("elementId")] public int? ElementId { get; set; }

    [Column("title")][JsonProperty("title")] public string? Title { get; set; }

    [Column("description")][JsonProperty("description")] public string? Description { get; set; }

    [Column("element_type_id")][JsonProperty("typeId")] public int TypeId { get; set; }

    [Column("citation")][JsonProperty("citation")] public string? Citation { get; set; }

    [Column("content", "json")][JsonProperty("content")] public string? Content { get; set; }

    [Column("external_source")][JsonProperty("externalSource")] public string? ExternalSource { get; set; }

    [Column("tags")][JsonProperty("tags")] public string[]? Tags { get; set; }

    [Column("library_folder_id")][JsonProperty("libraryFolderId")] public int? LibraryFolderId { get; set; }

    [Column("created_at")][JsonProperty("createdAt")] public string? CreatedAt { get; set; }

    [Column("updated_at")][JsonProperty("updatedAt")] public string? UpdatedAt { get; set; }

    [Column("created_by")][JsonProperty("createdBy")] public int? CreatedBy { get; set; }

    [Column("updated_by")][JsonProperty("updatedBy")] public int? UpdatedBy { get; set; }

    public IFormFile? FormFile { get; set; }

    public IElementType? ElementType { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Element)obj;

        return this.ElementId == other.ElementId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}