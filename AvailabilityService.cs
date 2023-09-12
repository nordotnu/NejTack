using Microsoft.Extensions.Logging;

class AvailabilityService : IAvailabilityService
{
  private const string notAvailableEndpoint = "https://service-api.studentconsulting.com/v1/employee/availability/save/wholeday?day={0}&status={1}";
  private readonly ISCService _scService;
  private readonly ILogger<IAvailabilityService> _logger;
  public AvailabilityService(IHttpClientFactory _httpFactory, ISCService scService, ILogger<IAvailabilityService> logger)
  {
    _logger = logger;
    _scService = scService;

  }

  public async Task<bool> NotAvailableTask()
  {
    var tries = 0;
    var tut = TimeUntilTomorrow();
    while (tries < 3)
    {
      _logger.LogInformation("Waiting {Tut} set unavailability.", tut.ToString());
      await Task.Delay(tut);
      _logger.LogInformation("Starting the setting procces...");
      var result = await PutNotAvailable();
      if (result)
      {
        tut = TimeUntilTomorrow();
        tries = 0;
      }
      else
      {
        _logger.LogWarning($"Retrying the setting procces ({tries + 1}/3).");
        tries++;
        tut = new TimeSpan(0);
      }

    }
    _logger.LogCritical($"Setting unavailable failed, closing the task.");
    return false;
  }

  private async Task<bool> PutNotAvailable()
  {
    var authenticated = await _scService.AuthenticateAsync();
    if (!authenticated)
    {
      return false;
    }
    var date = DateTime.Now.ToString("yyyy-MM-dd");
    string url = String.Format(notAvailableEndpoint, date, "NotAvailable");
    try
    {
      var httpMessage = await _scService.PutAsync(url);
      if (httpMessage.IsSuccessStatusCode)
      {
        _logger.LogInformation("Set the day {Date} successfully.", date);
        return true;
      }
      _logger.LogError("Failed to set the day {Date}.", date);
      _logger.LogInformation("Response code: {Code}", httpMessage.StatusCode);
      return false;
    }
    catch (System.Exception)
    {
      return false;
    }

  }
  static TimeSpan TimeUntilTomorrow()
  {
    DateTime now = DateTime.Now;
    DateTime tomorrow = now.AddDays(1).Date;
    TimeSpan timeUntilTomorrow = tomorrow - now;
    return timeUntilTomorrow;
  }

}
