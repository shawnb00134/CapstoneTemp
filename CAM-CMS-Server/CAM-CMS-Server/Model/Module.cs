using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class Module
{
    #region Properties

    public IEnumerable<ElementSet?>? ElementSets { get; set; }

    [Column("module_id")] public int ModuleId { get; set; }

    [Column("title")] public string? Title { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("survey_start_link")] public string? SurveyStartLink { get; set; }

    [Column("survey_end_link")] public string? SurveyEndLink { get; set; }

    [Column("contact_title")] public string? ContactTitle { get; set; }

    [Column("contact_number")] public string? ContactNumber { get; set; }

    [Column("thumbnail")] public string? Thumbnail { get; set; }

    [Column("tags")] public string[]? Tags { get; set; }

    [Column("display_title")] public string? DisplayTitle { get; set; }

    [Column("template_module_id")] public int? TemplateModuleId { get; set; }

    [Column("is_template")] public bool? IsTemplate { get; set; }

    [Column("publish_time")] public string? PublishedTime { get; set; }

    [Column("created_at")] public string? CreatedAt { get; set; }

    [Column("created_by")] public int? CreatedBy { get; set; }

    [Column("library_folder_id")] public int? LibraryFolderId { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Module)obj;

        return this.ModuleId == other.ModuleId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}