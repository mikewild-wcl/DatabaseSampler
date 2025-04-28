using Microsoft.AspNetCore;
using DatabaseSampler;

CreateWebHostBuilder(args).Build().Run();

static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
