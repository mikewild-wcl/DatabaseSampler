using DatabaseSampler.Extensions;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddConfiguration()
    .AddServices()
    .AddDatabases()
    .AddHttpClients();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//await app.Services.InitializeCosmosDb().ConfigureAwait(true);

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync().ConfigureAwait(true);
