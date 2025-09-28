using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Common.Generators.Models.ClassToGenerate;

namespace Common.Generators.Models;

internal class ClassToGenerate(string namespaceName,
    string className,
    IEnumerable<ConfigMapping> configs) : IEquatable<ClassToGenerate?>
{
    public string NamespaceName { get; } = namespaceName;
    public string ClassName { get; } = className;
    public IEnumerable<ConfigMapping> Configs { get; } = configs;

    public override bool Equals(object? obj)
    {
        return Equals(obj as ClassToGenerate);
    }

    public bool Equals(ClassToGenerate? other)
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

    internal sealed class ConfigMapping : IEquatable<ConfigMapping?>
    {
        public ConfigMapping(string entityTypeName, string configTypeName)
        {
            EntityTypeName = entityTypeName;
            ConfigTypeName = configTypeName;
        }

        public string EntityTypeName { get; }
        public string ConfigTypeName { get; }

        public bool Equals(ConfigMapping? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return EntityTypeName == other.EntityTypeName &&
                   ConfigTypeName == other.ConfigTypeName;
        }

        public override bool Equals(object? obj) =>
            Equals(obj as ConfigMapping);

        public override int GetHashCode() =>
           (EntityTypeName, ConfigTypeName).GetHashCode();

        public static bool operator ==(ConfigMapping? left, ConfigMapping? right) =>
            Equals(left, right);

        public static bool operator !=(ConfigMapping? left, ConfigMapping? right) =>
            !Equals(left, right);
    }
}