var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgres")
    .AddDatabase("jodwai-db");

var migrations = builder.AddProject<Projects.JodWai_MigrationService>("migrations")
    .WithReference(db)
    .WaitFor(db);

var backendApi = builder.AddProject<Projects.JodWai_Api>("jodwai-api")
    .WithReference(db)
    .WaitFor(migrations);

builder.AddViteApp("jodwai-web", "../../../frontend/JodWai-Web", "dev")
   .WithEnvironment("VITE_API_URL", 
   backendApi.GetEndpoint("https"));

builder.Build().Run();
