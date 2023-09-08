using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace NejTack
{
    class Program
    {
        static async Task Main()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();

            var username = config["username"];
            var password = config["password"];
            var secret = config["secret"];

            if (username == null || password == null || secret == null ){
                System.Console.WriteLine("Could NOT retrive the credintials from the settings");
                return;
            }

            var cookieContainer = new CookieContainer();
            // Setup a services
            var services = new ServiceCollection();
            services.AddHttpClient("scClient")
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        CookieContainer = new CookieContainer(),
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                        AllowAutoRedirect = true
                    });
            var serviceProvider = services.BuildServiceProvider();


            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();


            var authenticator = new AuthService(httpClientFactory, username, password, secret);
            System.Console.WriteLine("MAIN:" + await authenticator.AuthenticateAsync());
            System.Console.WriteLine("MAIN:" + await authenticator.AuthenticateAsync());

        }
    }
}
