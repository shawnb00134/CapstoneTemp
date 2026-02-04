using CAMCMSServer.Database.Mappers;

namespace CAMCMSServer.Model;

public class LibraryFolder
{
    #region Properties

    public List<Module>? Modules { get; set; }

    public List<Element>? Elements { get; set; }

    [Column("library_folder_id")] public int LibraryFolderId { get; set; }

    [Column("name")] public string? Name { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("created_by")] public int? CreatedBy { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (LibraryFolder)obj;

        return this.LibraryFolderId == other.LibraryFolderId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}