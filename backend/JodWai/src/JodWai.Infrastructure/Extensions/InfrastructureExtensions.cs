using JodWai.Application.Interfaces;
using JodWai.Infrastructure.Parsing;
using JodWai.Infrastructure.Persistence;
using JodWai.Infrastructure.Persistence.Repositories;
using JodWai.Infrastructure.Persistence.Workers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JodWai.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
                configuration.GetConnectionString("jodwai-db")));

        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<INoteLinkParser, WikiStyleNoteLinkParser>();


        return services;
    }

    public static IServiceCollection AddDatabaseMigration(
        this IServiceCollection services)
    {
        services.AddHostedService<MigrationWorker>();
        return services;
    }
}
