var builder = DistributedApplication.CreateBuilder(args);

var minicrud_api = builder.AddProject<Projects.MiniCRUD_API>("minicrud-api");

var minicrud_webapp = builder.AddProject<Projects.MiniCRUD_WebApp>("minicrud-webapp")
    .WithReference(minicrud_api);

builder.Build().Run();
