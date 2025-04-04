using Microsoft.Extensions.DependencyInjection;

namespace MyLibrary.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration => //TODO-Feature: Implement custom solution instead of MediatR
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // configuration.AddOpenBehavior(typeof(LoggingBehaviour<,>)); //TODO-Feature: Logging behaviour

            // configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>)); //TODO-Feature: Validation behaviour
        });

        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true); //TODO-Feature: Validation behaviour

        return services;
    }
}