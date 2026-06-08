using Microsoft.Extensions.DependencyInjection;

namespace JodWai.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ApplicationExtensions).Assembly);
        });

        return services;
    }
}
