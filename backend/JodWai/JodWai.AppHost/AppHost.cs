var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgres")
    .AddDatabase("jodwai-db");

var migrations = builder.AddProject<Projects.JodWai_MigrationService>("migrations")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.JodWai_Api>("jodwai-api")
    .WithReference(db)
    .WaitFor(migrations);

builder.Build().Run();
