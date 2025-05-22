using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyLibrary.Infrastructure.Abstraction.Entity.Configuration;

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Domain.Abstraction.Entity.Entity
{
    protected void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EntityStatus);
    }

    public abstract void Configure(EntityTypeBuilder<TEntity> builder);
}