using System.Diagnostics.CodeAnalysis;

namespace CAMCMSServer.Database.Mappers;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    #region Properties

    public string Name { get; set; }

    public string? TypeName { get; set; }

    #endregion

    #region Constructors

    public ColumnAttribute(string name)
    {
        this.Name = name;
    }

    public ColumnAttribute(string name, string? typeName) : this(name)
    {
        this.TypeName = typeName;
    }

    #endregion
}