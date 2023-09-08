class AvailabilityService : IAvailabilityService
{
  private const string notAvailableEndpoint = "https://service-api.studentconsulting.com/v1/employee/availability/save/wholeday?day={0}&status={1}";
  private readonly IAuthService _authService;
  public AvailabilityService(IHttpClientFactory _httpFactory, IAuthService authService)
  {
    _authService = authService;
  }

  public async Task<bool> NotAvailableTask()
  {
    var tries = 0;
    var tut = TimeUntilTomorrow();
    while (tries < 3)
    {
      System.Console.WriteLine($"Waiting {tut.ToString()} set unavailability.");
      await Task.Delay(tut);
      System.Console.WriteLine("Starting the setting procces...");
      var result = await PutNotAvailable();
      if (result)
      {
        tut = TimeUntilTomorrow();
        tries = 0;
      }
      else
      {
        System.Console.WriteLine($"Retrying the setting procces ({tries + 1}/3).");
        tries++;
        tut = new TimeSpan(0);
      }

    }
    System.Console.WriteLine($"Setting unavailable failed, closing the task.");
    return false;
  }

  private async Task<bool> PutNotAvailable()
  {
    var authenticated = await _authService.AuthenticateAsync();
    if (!authenticated)
    {
      return false;
    }
    var date = DateTime.Now.ToString("yyyy-MM-dd");
    string url = String.Format(notAvailableEndpoint, date, "NotAvailable");
    try
    {
      var httpMessage = await _authService.PutAsync(url);
      if (httpMessage.IsSuccessStatusCode)
      {
        System.Console.WriteLine($"Set the day {date} successfully.");
        return true;
      }
      System.Console.WriteLine($"Failed to set the day {date}.");
      System.Console.WriteLine($"Response code: {httpMessage.StatusCode.ToString()}");
      return false;
    }
    catch (System.Exception)
    {
      return false;
    }

  }
  TimeSpan TimeUntilTomorrow()
  {
    DateTime now = DateTime.Now;
    DateTime tomorrow = now.AddDays(1).Date;
    TimeSpan timeUntilTomorrow = tomorrow - now;
    return new TimeSpan(0, 0, 10);
    return timeUntilTomorrow;
  }

}
