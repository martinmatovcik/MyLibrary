using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Infrastructure.Database;

namespace MyLibrary.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<MyLibraryDbContext>(options =>
        {
            options.UseNpgsql(connectionString, postgre => postgre.UseNodaTime())
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging();
        });
        
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<MyLibraryDbContext>());

        services.AddRepositories();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        // services.AddScoped<ISymbolRepository, SymbolRepository>();
    }
}