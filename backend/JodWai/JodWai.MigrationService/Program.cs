using JodWai.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddDatabaseMigration();

var app = builder.Build();
app.Run();
