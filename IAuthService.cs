interface IAuthService
{
  Task<bool> AuthenticateAsync();

  Task<bool> GetCookieAsync(bool refresh = false);
  Task<bool> IsAuthenticated();

  Task<HttpResponseMessage> PutAsync(string url);

}
