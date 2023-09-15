interface ISCService
{
    Task<bool> AuthenticateAsync();
    Task<bool> GetCookieAsync(bool refresh = false);
    Task<bool> IsAuthenticated();
    Task<HttpResponseMessage> PutAsync(string url);
    Task<string> GetInquiriesJson();
    Task<bool> PostResponse(string jsonData);
    Task<bool> PutNotAvailable();
}
