using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<CareerCompass_Api>("webapi");

builder.Build().Run();