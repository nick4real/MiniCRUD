var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

void ConfigureReverseProxy()
{
    var reverseProxyClusters = builder.Configuration.GetSection("ReverseProxy:Clusters");

    foreach (var cluster in reverseProxyClusters.GetChildren())
    {
        var clusterName = cluster.Key.Replace("-cluster", string.Empty);

        var tmp2 = builder.Configuration.GetSection($"ReverseProxy:Clusters:{cluster.Key}:Destinations:destination-1:Address");
        var destinationUrl2 = Environment.GetEnvironmentVariable($"services__{clusterName}__http__0");
        tmp2.Value = destinationUrl2;
    }
}

ConfigureReverseProxy();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapReverseProxy();

app.Run();