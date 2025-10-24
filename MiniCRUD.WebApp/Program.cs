using Microsoft.AspNetCore.HttpOverrides;
using MiniCRUD.WebApp.Components;
using MiniCRUD.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<HttpClientService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost,

    // Only use 1 hop in Azure
    ForwardLimit = 1
});

app.Use(async (context, next) =>
{
    // Fix for scheme
    var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
    if (!string.IsNullOrEmpty(forwardedProto))
    {
        context.Request.Scheme = forwardedProto;
    }

    // Fix for host
    var forwardedHost = context.Request.Headers["X-Forwarded-Host"].FirstOrDefault();
    if (!string.IsNullOrEmpty(forwardedHost))
    {
        context.Request.Host = new HostString(forwardedHost);
    }

    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();