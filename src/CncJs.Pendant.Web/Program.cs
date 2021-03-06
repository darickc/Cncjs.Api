using System.Text.Json;
using CncJs.Api;
using CncJs.Pendant.Web.Shared.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Configuration.AddJsonFile("settings.json", true);

CncJsOptions options = new CncJsOptions();
var secretFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cncrc");
if (File.Exists(secretFile))
{
    var json = await File.ReadAllTextAsync(secretFile);
    options = JsonSerializer.Deserialize<CncJsOptions>(json, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }) ?? new CncJsOptions();
}
builder.Configuration.Bind(options);

builder.Services.AddSingleton(options);
builder.Services.AddSingleton<CncJsClient>();
builder.Services.AddScoped<JavascriptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}



//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
