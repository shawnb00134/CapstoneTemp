using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class PackageFolderModule
{
    #region Properties

    [Column("package_folder_module_id")] public int PackageFolderModuleId { get; set; }

    [Column("package_folder_id")] public int PackageFolderId { get; set; }

    [Column("published_module_id")] public int PublishedModuleId { get; set; }

    [Column("order_in_folder")] public int OrderInFolder { get; set; }
    [Column("editable")] public bool Editable { get; set; }

    [Column("cache", "json")] public string? Cache { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = obj as PackageFolderModule;
        return this.PackageFolderModuleId == other.PackageFolderModuleId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}