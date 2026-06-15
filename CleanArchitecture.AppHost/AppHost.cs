var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArchitecture>("cleanarchitecture");

builder.Build().Run();
