using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchRankChecker.Api.Infrastructure.Persistence;

public interface IEntityTypeProvider
{
    IEnumerable<Type> GetEntityTypes(Type baseType);
    IEnumerable<Type> GetEntityTypeConfigurations(Type[] types, Type entityTypeConfiguration);
}

public class EntityTypeProvider : IEntityTypeProvider
{
    private static IDictionary<Type, IList<Type>> _types = new Dictionary<Type, IList<Type>>();

    public IEnumerable<Type> GetEntityTypeConfigurations(Type[] types, Type entityTypeConfiguration)
    {
        if (_types.Keys.Contains(entityTypeConfiguration)) return _types[entityTypeConfiguration];

        types = types
            .Where(type => type.BaseType != null)
            .Where(type => type.BaseType!.IsGenericType)
            .Where(type => type.BaseType!.GetGenericTypeDefinition() == entityTypeConfiguration)
            .ToArray();

        _types.Add(entityTypeConfiguration, types);

        return types;
    }

    public IEnumerable<Type> GetEntityTypes(Type baseType)
    {
        if (_types.Keys.Contains(baseType)) return _types[baseType];

        var types = baseType.Assembly.GetTypes()
            .Where(type => !string.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null)
            .Where(type => type.BaseType == baseType)
            .ToList();

        _types.Add(baseType, types);

        return types;
    }
}


