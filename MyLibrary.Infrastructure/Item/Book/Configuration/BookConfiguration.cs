using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Infrastructure.Item.Configuration;

namespace MyLibrary.Infrastructure.Item.Book.Configuration;

public class BookConfiguration : ItemConfiguration<Domain.Item.Book.Book>
{
    public override void Configure(EntityTypeBuilder<Domain.Item.Book.Book> builder)
    {
        builder = ConfigureItem(builder);

        builder.Property(x => x.Author);
        builder.Property(x => x.Year).HasDefaultValue(1990);
        builder.Property(x => x.Isbn).HasMaxLength(22);
    }
}