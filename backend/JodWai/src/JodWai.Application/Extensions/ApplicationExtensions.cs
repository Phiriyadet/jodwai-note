using JodWai.Application.Behaviors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using JodWai.Application.Interfaces;
using JodWai.Application.Services;

namespace JodWai.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(ApplicationExtensions).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddScoped<INoteLinkResolver, NoteLinkResolver>();

        return services;
    }
}
