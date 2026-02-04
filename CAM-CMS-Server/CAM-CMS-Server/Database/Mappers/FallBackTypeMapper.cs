using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Dapper;

namespace CAMCMSServer.Database.Mappers;

[ExcludeFromCodeCoverage]
public class FallBackTypeMapper : SqlMapper.ITypeMap
{
    #region Data members

    private readonly IEnumerable<SqlMapper.ITypeMap> mappers;

    #endregion

    #region Constructors

    public FallBackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
    {
        this.mappers = mappers;
    }

    #endregion

    #region Methods

    public ConstructorInfo FindConstructor(string[] names, Type[] types)
    {
        foreach (var mapper in this.mappers)
        {
            try
            {
                var result = mapper.FindConstructor(names, types);

                if (result != null)
                {
                    return result;
                }
            }
            catch (NotImplementedException)
            {
                // the CustomPropertyTypeMap only supports a no-args
                // constructor and throws a not implemented exception.
                // to work around that, catch and ignore.
            }
        }

        return null!;
    }

    public ConstructorInfo FindExplicitConstructor()
    {
        return this.mappers.Select(m => m.FindExplicitConstructor())
            .FirstOrDefault(result => result != null)!;
    }

    public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
    {
        foreach (var mapper in this.mappers)
        {
            try
            {
                var result = mapper.GetConstructorParameter(constructor, columnName);

                if (result != null)
                {
                    return result;
                }
            }
            catch (NotImplementedException)
            {
                // the CustomPropertyTypeMap only supports a no-args
                // constructor and throws a not implemented exception.
                // to work around that, catch and ignore.
            }
        }

        return null!;
    }

    public SqlMapper.IMemberMap GetMember(string columnName)
    {
        foreach (var mapper in this.mappers)
        {
            try
            {
                var result = mapper.GetMember(columnName);

                if (result != null)
                {
                    return result;
                }
            }
            catch (NotImplementedException)
            {
                // the CustomPropertyTypeMap only supports a no-args
                // constructor and throws a not implemented exception.
                // to work around that, catch and ignore.
            }
        }

        return null!;
    }

    #endregion
}