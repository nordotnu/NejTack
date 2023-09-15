using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;

namespace Services
{
  class SCService : ISCService
  {
    const string INQUIRY_URL = "https://service-api.studentconsulting.com/v1/employee/inquiry/notresponded";
    const string RESPONSE_URL = "https://service-api.studentconsulting.com/v1/employee/inquiry/response";
    const string TOKEN_URL = "https://id.studentconsulting.com/connect/token";
    const string LOGIN_URL = "https://employee.studentconsulting.com/login";
    const string AVAILABLE_URL = "https://service-api.studentconsulting.com/v1/employee/availability/save/wholeday?day={0}&status={1}";

    private readonly HttpClient _client;
    private readonly ILogger<ISCService> _logger;
    private readonly IConfiguration? _config;
    private readonly string _username;
    private readonly string _password;
    private readonly string _secret;
    private string _refreshToken;

    public SCService(IHttpClientFactory _httpFactory, ILogger<ISCService> logger, IConfiguration config)
    {
      _logger = logger;
      _client = _httpFactory.CreateClient("scClient");
      _username = config["username"]!;
      _password = config["password"]!;
      _secret = config["secret"]!;
      if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_secret))
      {
        _logger.LogCritical("Unvalid configuration!");
        throw new InvalidDataException("Could NOT find credintials in the appsetting.json!");
      }
      _refreshToken = "";
    }

    public async Task<bool> AuthenticateAsync()
    {
      bool auth = await IsAuthenticated();
      for (int i = 0; i < 3; i++)
      {
        if (!auth)
        {
          _logger.LogInformation("Trying to login, ({Tries}) with refresh-token: {Status}",
                                i + 1, !String.IsNullOrEmpty(_refreshToken));
          auth = await GetCookieAsync(!String.IsNullOrEmpty(_refreshToken));
        }
        else
        {
          _logger.LogInformation("Logged in!");
          return true;
        }
      }
      _logger.LogCritical("Failed to login");
      _refreshToken = "";
      _client.DefaultRequestHeaders.Authorization = null;
      return false;
    }

    public async Task<bool> GetCookieAsync(bool refresh = false)
    {
      // Send a get request to login page to retrive Session cookie.
      var url = LOGIN_URL;
      var httpMessage = new HttpRequestMessage(HttpMethod.Get, url);
      try
      {
        var content = await _client.SendAsync(httpMessage);
      }
      catch (System.Exception)
      {
        return false;
      }

      // Create a payload with the user credintials.
      var data = new List<KeyValuePair<string, string>>();
      if (refresh)
      {
        data.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
        data.Add(new KeyValuePair<string, string>("refresh_token", _refreshToken));
      }
      else
      {
        data.Add(new KeyValuePair<string, string>("username", _username));
        data.Add(new KeyValuePair<string, string>("password", _password));
        data.Add(new KeyValuePair<string, string>("grant_type", "password"));
      }
      data.Add(new KeyValuePair<string, string>("client_id", "external-fabriken-login"));
      data.Add(new KeyValuePair<string, string>("client_secret", _secret));
      var cred = new FormUrlEncodedContent(data);
      try
      {
        // Request the token
        var content = await _client.PostAsync(
            TOKEN_URL,
            cred);
        var html = await content.Content.ReadAsStringAsync();

        // Add the token to the default headers
        string pattern = @"(?<=""access_token"":"")[^""]+";
        Match match = Regex.Match(html, pattern);
        var token = match.Groups[0].Value;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


        // Save the token and refresh token if the token is valid
        if (await IsAuthenticated())
        {
          pattern = @"(?<=""refresh_token"":"")[^""]+";
          match = Regex.Match(html, pattern);
          _refreshToken = match.Groups[0].Value;
          return true;
        }
        return false;
      }
      catch (System.Exception)
      {
        return false;
      }
    }

    public async Task<bool> IsAuthenticated()
    {
      _logger.LogInformation("Checking for authentication.");
      var checkCookie = await _client.GetAsync(INQUIRY_URL);
      return checkCookie.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> PutAsync(string url)
    {
      return await _client.PutAsync(url, null);
    }

    public async Task<string> GetInquiriesJson()
    {
      var isAuthenticated = await AuthenticateAsync();
      if (isAuthenticated)
      {
        var httpResponse = await _client.GetAsync(INQUIRY_URL);
        if (httpResponse.IsSuccessStatusCode)
        {
          return await httpResponse.Content.ReadAsStringAsync();
        }
      }
      return "";
    }

    public async Task<bool> PostResponse(string jsonData)
    {
      var isAuthenticated = await AuthenticateAsync();
      if (isAuthenticated)
      {
        var data = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var httpResponse = await _client.PostAsync(RESPONSE_URL, data);
        if (httpResponse.IsSuccessStatusCode)
        {
          _logger.LogInformation("Posted inquiry successfully");
          return true;
        }
        else
        {
          _logger.LogWarning("Received non success code while sending response. {Code}",
           httpResponse.StatusCode);
        }
      }
      return false;
    }

    public async Task<bool> PutNotAvailable()
    {
      var isAuthenticated = await AuthenticateAsync();
      if (!isAuthenticated)
      {
        return false;
      }
      var date = DateTime.Now.ToString("yyyy-MM-dd");
      string url = String.Format(AVAILABLE_URL, date, "NotAvailable");

      var httpMessage = await _client.PutAsync(url, null);
      if (httpMessage.IsSuccessStatusCode)
      {
        _logger.LogInformation("Set the day {Date} successfully.", date);
        return true;
      }
      _logger.LogError("Failed to set the day {Date}.", date);
      _logger.LogInformation("Response code: {Code}", httpMessage.StatusCode);
      return false;
    }
  }
}
