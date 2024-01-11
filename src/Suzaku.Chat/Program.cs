using Microsoft.Extensions.FileProviders;
using Suzaku.Chat;
using Suzaku.Chat.Client.Pages;
using Suzaku.Chat.Components;
using Suzaku.Chat.Models;
using Suzaku.Chat.Services;
using Suzaku.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.Configure<MqttConfiguration>(builder.Configuration.GetSection("Mqtt"));
builder.Services.Configure<UserConfiguration>(builder.Configuration.GetSection("User"));
builder.Services.AddSingleton<ChatHistory>();
builder.Services.AddSingleton<MqttService>();
builder.Services.AddScoped<ChatCommandService>();
builder.Services.AddScoped<FileHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "uploads")
        ),
        RequestPath = "/uploads"
    }
);
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

await app.Services.GetRequiredService<MqttService>().InitializeAsync();

app.Run();
