using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions.Internal;

namespace AuthAPI.Data;



public interface IEntityConfigurationAggregator
{
    /// <summary>
    /// Applies entity configurations to the given model builder using the specified name rewriter.
    /// </summary>
    /// <param name="modelBuilder">The EF Core ModelBuilder.</param>
    /// <param name="nameRewriter">The name rewriter for table/column naming conventions.</param>
    void ApplyAutoConfigurations(ModelBuilder modelBuilder, INameRewriter nameRewriter);
}

