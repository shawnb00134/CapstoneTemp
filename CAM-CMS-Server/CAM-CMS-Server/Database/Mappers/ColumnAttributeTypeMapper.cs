using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace CAMCMSServer.Database.Mappers;

[ExcludeFromCodeCoverage]
public class ColumnAttributeTypeMapper<T> : FallBackTypeMapper
{
    #region Constructors

    public ColumnAttributeTypeMapper()
        : base(new SqlMapper.ITypeMap[]
        {
            new CustomPropertyTypeMap(typeof(T),
                (type, columnName) =>
                    type.GetProperties().FirstOrDefault(prop =>
                        prop.GetCustomAttributes(false)
                            .OfType<ColumnAttribute>()
                            .Any(attribute => attribute.Name == columnName)
                    )
            ),
            new DefaultTypeMap(typeof(T))
        })
    {
    }

    #endregion
}