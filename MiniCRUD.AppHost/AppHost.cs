var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MiniCRUD_API>("minicrud-api");

builder.AddProject<Projects.MiniCRUD_WebApp>("minicrud-webapp");

builder.Build().Run();
