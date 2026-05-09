var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.JodWai_Api>("jodwai-api");

builder.Build().Run();
