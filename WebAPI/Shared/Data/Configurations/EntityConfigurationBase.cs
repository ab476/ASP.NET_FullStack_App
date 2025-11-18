using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Data.Configurations;
public abstract class EntityConfigurationBase<TEntity>(INameHelper nameRewriter) 
    : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    protected INameHelper NameRewriter { get; }
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
