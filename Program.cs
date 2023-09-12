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

      var builder = new ConfigurationBuilder();
      BuildConfig(builder);

      var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                  services.AddHttpClient("scClient")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                  CookieContainer = new CookieContainer(),
                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                  AllowAutoRedirect = true
                });
                  services.AddSingleton<ISCService, SCService>();
                  services.AddSingleton<IAvailabilityService, AvailabilityService>();

                })
                .Build();
      var svc = ActivatorUtilities.CreateInstance<AvailabilityService>(host.Services);
      await svc.NotAvailableTask();

    }
    static void BuildConfig(IConfigurationBuilder builder)
    {
      builder.SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }
  }
}
