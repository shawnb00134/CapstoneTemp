using System.ComponentModel.DataAnnotations.Schema;

namespace CAMCMSServer.Model;

public class OrganizationPackage
{
    #region Properties

    [Column("organization_id")] public int OrganizationId { get; set; }

    [Column("package_id")] public int PackageId { get; set; }

    #endregion

    #region Methods

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (OrganizationPackage)obj;

        return this.OrganizationId == other.OrganizationId && this.PackageId == other.PackageId;
    }

    public override int GetHashCode()
    {
        return this.PackageId.GetHashCode() + this.OrganizationId.GetHashCode();
    }

    #endregion
}