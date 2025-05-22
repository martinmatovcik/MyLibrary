using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyLibrary.Infrastructure.Data;

sealed internal class MyLibraryDbContextFactory(IPublisher publisher) : IDesignTimeDbContextFactory<MyLibraryDbContext>
{
    public MyLibraryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>();
        optionsBuilder.UseNpgsql("for-migration-only").UseSnakeCaseNamingConvention().EnableSensitiveDataLogging();
        return new MyLibraryDbContext(optionsBuilder.Options, publisher);
    }
}