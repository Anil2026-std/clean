var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArchitecture>("cleanarchitecture");

builder.AddProject<Projects.Frontend>("frontend");

builder.AddProject<Projects.ImageServer>("imageserver");

builder.Build().Run();
