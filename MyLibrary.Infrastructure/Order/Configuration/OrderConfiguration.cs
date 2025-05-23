using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Infrastructure.Abstraction.Entity.Configuration;

namespace MyLibrary.Infrastructure.Order.Configuration;

public class OrderConfiguration : EntityConfiguration<Domain.Order.Order>
{
    public override void Configure(EntityTypeBuilder<Domain.Order.Order> builder)
    {
        builder.OwnsMany(x => x.Items, builderInternal =>
        {
            builderInternal.Property(y => y.ItemId);
            builderInternal.Property(y => y.Name);
            builderInternal.Property(y => y.Owner);
        });
        builder.Property(x => x.ItemsOwner);
        builder.Property(x => x.Renter);
        builder.Property(x => x.Status);
        builder.Property(x => x.PickUpDateTime);
        builder.Property(x => x.PlannedReturnDate);
        builder.Property(x => x.Note);
    }
}