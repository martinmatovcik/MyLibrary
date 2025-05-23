using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Infrastructure.Abstraction.Entity.Configuration;

namespace MyLibrary.Infrastructure.Item.Configuration;

public abstract class ItemConfiguration<T> : EntityConfiguration<T> where T : Domain.Item.Abstraction.Item
{
    public EntityTypeBuilder<T> ConfigureItem(EntityTypeBuilder<T> builder)
    {
        builder = ConfigureEntity(builder);
        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
        builder.Property(x => x.Owner);
        builder.Property(x => x.Renter);
        // builder.Property(x => x.History);
        builder.Property(x => x.Status);

        return builder;
    }
}