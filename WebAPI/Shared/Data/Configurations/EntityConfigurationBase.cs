using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Data.Configurations;
public abstract class EntityConfigurationBase<TEntity>(INameRewriter nameRewriter) 
    : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    protected INameRewriter NameRewriter { get; }
        = nameRewriter
          ?? throw new ArgumentNullException(nameof(nameRewriter));

    /// <summary>
    /// Shortcut for <c>nameRewriter.RewriteName</c>.
    /// </summary>
    protected string Rewrite(string name)
    {
        return NameRewriter.RewriteName(name);
    }

    public abstract void Configure(EntityTypeBuilder<TEntity> builder);
}
