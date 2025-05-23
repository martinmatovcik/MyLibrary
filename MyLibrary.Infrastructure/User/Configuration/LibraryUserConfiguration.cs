using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.User;
using MyLibrary.Infrastructure.Abstraction.Entity.Configuration;

namespace MyLibrary.Infrastructure.User.Configuration;

public class LibraryUserConfiguration : EntityConfiguration<LibraryUser>
{
    public override void Configure(EntityTypeBuilder<LibraryUser> builder)
    {
        builder.OwnsOne(x => x.Details, builderInternal =>
        {
            builderInternal.Property(y => y.Email);
            builderInternal.Property(y => y.Username);
            builderInternal.Property(y => y.Password);
            builderInternal.Property(y => y.FirstName);
            builderInternal.Property(y => y.LastName);
            builderInternal.Property(y => y.PhoneNumber);
        });

        builder.OwnsMany(x => x.OwnedItems, builderInternal =>
        {
            builderInternal.Property(y => y.ItemId);
            builderInternal.Property(y => y.Name);
            builderInternal.Property(y => y.Owner);
        });

        builder.OwnsMany(x => x.Orders, builderInternal =>
        {
            builderInternal.Property(y => y.OrderId);
            builderInternal.OwnsMany(x => x.Items, builderInternal2 =>
            {
                builderInternal2.Property(y => y.ItemId);
                builderInternal2.Property(y => y.Name);
                builderInternal2.Property(y => y.Owner);
            });
            builderInternal.Property(y => y.ItemsOwner);
        });
    }
}