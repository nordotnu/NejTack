using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services
{
  class AvailabilityService : IAvailabilityService
  {
    private readonly ISCService _scService;
    private readonly ILogger<IAvailabilityService> _logger;
    private readonly IConfiguration _config;
    public AvailabilityService(ISCService scService, IConfiguration config, ILogger<IAvailabilityService> logger)
    {
      _logger = logger;
      _config = config;
      _scService = scService;
    }

    public async Task<bool> NotAvailableTask()
    {
      var run = _config.GetSection("availability").Get<bool>();
      if(!run) {
        return false;
      }
      _logger.LogInformation("Running the availability service.");
      var tries = 0;
      var tut = TimeUntilTomorrow();
      while (tries < 3)
      {
        _logger.LogInformation("Waiting {Tut} set unavailability.", tut.ToString());
        await Task.Delay(tut);
        _logger.LogInformation("Starting the setting procces...");
        var result = await _scService.PutNotAvailable();
        if (result)
        {
          tut = TimeUntilTomorrow();
          tries = 0;
        }
        else
        {
          _logger.LogWarning("Retrying the setting procces ({Tries}/3).", (tries + 1));
          tries++;
          tut = new TimeSpan(0);
        }

      }
      _logger.LogCritical($"Setting unavailable failed, closing the task.");
      return false;
    }


    static TimeSpan TimeUntilTomorrow()
    {
      DateTime now = DateTime.Now;
      DateTime tomorrow = now.AddDays(1).Date;
      TimeSpan timeUntilTomorrow = tomorrow - now;
      return timeUntilTomorrow;
    }

  }
}