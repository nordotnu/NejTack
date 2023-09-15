using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Utilities;
using Models;

namespace Services
{
  class AutoResponseService : IAutoResponseService
  {
    private readonly ILogger<AutoResponseService> _logger;
    private readonly IConfiguration _config;
    private readonly ISCService _scService;

    public AutoResponseService(ILogger<AutoResponseService> logger, IConfiguration config, ISCService sCService)
    {
      _logger = logger;
      _config = config;
      _scService = sCService;
    }
    public async Task Run()
    {
      var source = new CancellationTokenSource();
      _config.GetReloadToken().RegisterChangeCallback(_ => { source.Cancel(); }, null);
      while (true)
      {
        var arConfig = GetAutoResponseConfig();
        if (arConfig.Status)
        {
          var json = await _scService.GetInquiriesJson();
          var inquiries = DeserializeInquiries(json);
          var acceptInquiries = new List<Inquiry>();
          for (int i = 0; i < inquiries.Count; i++)
          {
            var inquiry = inquiries[i];
            for (int j = 0; j < arConfig.Accept.Count; j++)
            {
              var accept = arConfig.Accept[j];
              var sameStart = accept.Start.CompareTo(inquiry.Interval.Start) == 0;
              var sameEnd = accept.End.CompareTo(inquiry.Interval.End) == 0;
              if (sameStart && sameEnd)
              {
                acceptInquiries.Add(inquiry);
              }
            }
          }
          foreach (var i in acceptInquiries)
          {
            var j = SerializerResponse(i);
            _logger.LogError("Accepted: {Yes}", j);
          }
        }
        try
        {
          await Task.Delay(100000, source.Token);
        }
        catch (TaskCanceledException)
        {
          _logger.LogInformation("Configuration reloaded.");
          source.Dispose();
          source = new CancellationTokenSource();
        }

      }
    }


    private AutoResponseConfig GetAutoResponseConfig()
    {
      var arConfig = _config.GetSection("autoresponse").Get<AutoResponseConfig>()!;
      if (arConfig == null)
      {
        _logger.LogCritical("Unvalid configuration!");
        throw new InvalidDataException("Could NOT find credintials in the appsetting.json!");
      }
      return arConfig;
    }

    private string SerializerResponse(Inquiry inquiry)
    {
      Response response = new()
      {
        BlockId = inquiry.BlockId,
        Availability = new(),
        Inquiries = new List<ResponseInquiry> {
          new() {
            Id = inquiry.Id,
            Response = "Accepted",
            AcceptedInterval = new() {
              Start = inquiry.Interval.Start.ToUniversalTime(),
              End = inquiry.Interval.End.ToUniversalTime()
            }
          }
        }
      };
      var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = new LowerCaseNamingPolicy() };
      string jsonString = JsonSerializer.Serialize(response, options);
      return jsonString;
    }

    private List<Inquiry> DeserializeInquiries(string json)
    {
      try
      {
        JsonNode? rootNode = JsonObject.Parse(json);
        JsonNode? dataNode = rootNode!["data"];
        var count = dataNode!.AsArray().Count;
        List<Inquiry> inquiries = new();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        for (int i = 0; i < count; i++)
        {
          Inquiry? inquiry = dataNode[i]!["inquiries"]![0].Deserialize<Inquiry>(options);
          if (inquiry == null)
          {
            throw new InvalidDataException();
          }
          inquiries.Add(inquiry!);
        }
        return inquiries;
      }
      catch (Exception)
      {
        _logger.LogError("Could NOT deserialize JSON:\n{JsonString}\n --END JSON--", json);
        throw new InvalidDataException();
      }
    }

  }
}