using System;
using System.Collections.Generic;
using System.Linq;
using static Common.Generators.Models.TargetClassInfo;

namespace Common.Generators.Models;

internal class TargetClassInfo(string namespaceName,
    string className,
    IEnumerable<EntityConfigMapping> configs) : IEquatable<TargetClassInfo?>
{
    public string NamespaceName { get; } = namespaceName;
    public string ClassName { get; } = className;
    public IEnumerable<EntityConfigMapping> Configs { get; } = configs;

    public override bool Equals(object? obj)
    {
        return Equals(obj as TargetClassInfo);
    }

    public bool Equals(TargetClassInfo? other)
    {
        return other is not null &&
               NamespaceName == other.NamespaceName &&
               ClassName == other.ClassName &&
               Configs.SequenceEqual(other.Configs);
    }

    public override int GetHashCode()
    {
        int hashCode = -39551721;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamespaceName);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClassName);
        return hashCode;
    }

    internal sealed class EntityConfigMapping : IEquatable<EntityConfigMapping?>
    {
        public EntityConfigMapping(string entityTypeName, string configTypeName)
        {
            EntityTypeName = entityTypeName;
            ConfigTypeName = configTypeName;
        }

        public string EntityTypeName { get; }
        public string ConfigTypeName { get; }

        public bool Equals(EntityConfigMapping? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return EntityTypeName == other.EntityTypeName &&
                   ConfigTypeName == other.ConfigTypeName;
        }

        public override bool Equals(object? obj) =>
            Equals(obj as EntityConfigMapping);

        public override int GetHashCode() =>
           (EntityTypeName, ConfigTypeName).GetHashCode();

        public static bool operator ==(EntityConfigMapping? left, EntityConfigMapping? right) =>
            Equals(left, right);

        public static bool operator !=(EntityConfigMapping? left, EntityConfigMapping? right) =>
            !Equals(left, right);
    }
}