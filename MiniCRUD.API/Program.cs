using Microsoft.EntityFrameworkCore;
using MiniCRUD.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, o =>
    {
        o.EnableRetryOnFailure();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await using (var serviceScope = app.Services.CreateAsyncScope())
await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();
