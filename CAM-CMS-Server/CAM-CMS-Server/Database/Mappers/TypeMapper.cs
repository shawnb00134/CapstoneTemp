using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace CAMCMSServer.Database.Mappers;

[ExcludeFromCodeCoverage]
public static class TypeMapper
{
    #region Methods

    public static void Initialize(string @namespace)
    {
        var types = from assembly in AppDomain.CurrentDomain.GetAssemblies().ToList()
            from type in assembly.GetTypes()
            where type.IsClass && type.Namespace == @namespace
            select type;

        types.ToList().ForEach(type =>
        {
            var mapper = (SqlMapper.ITypeMap)Activator
                .CreateInstance(typeof(ColumnAttributeTypeMapper<>)
                    .MakeGenericType(type))!;
            SqlMapper.SetTypeMap(type, mapper);
        });
    }

    #endregion
}