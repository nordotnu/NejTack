using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NejTack
{
    class Program
    {
        static async Task Main()
        {
            var confBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true);
            var config = confBuilder.Build();

            var username = config["username"];
            var password = config["password"];
            var secret = config["secret"];

            if (username == null || password == null || secret == null)
            {
                System.Console.WriteLine("Could NOT retrive the credintials from the settings");
                return;
            }
            var builder = Host.CreateApplicationBuilder();
            var cookieContainer = new CookieContainer();

            builder.Services.AddHttpClient("scClient")
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        CookieContainer = cookieContainer,
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                        AllowAutoRedirect = true
                    });
            builder.Services.AddSingleton<IAuthService>(serviceProvider => new AuthService(
                serviceProvider.GetService<IHttpClientFactory>()!, username, password, secret));
            builder.Services.AddSingleton<IAvailabilityService, AvailabilityService>();
            using var host = builder.Build();
            var hostProvider = host.Services;
            using var serviceScope = hostProvider.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var authService = provider.GetRequiredService<IAuthService>();
            var availabilityService = provider.GetRequiredService<IAvailabilityService>();
            await availabilityService.NotAvailableTask();

            await host.RunAsync();

        }
    }
}
