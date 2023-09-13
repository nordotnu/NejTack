using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Utilities;
using Models;

namespace Services
{
  class AutoResponseService : IAutoResponseService
  {
    private readonly ILogger<ISCService> _logger;

    public AutoResponseService(ILogger<ISCService> logger)
    {
      _logger = logger;
    }
    public void Run()
    {
      using StreamReader r = new StreamReader("file.json");
      string json = r.ReadToEnd();
      List<Inquiry> inquiries = DeserializeInquiries(json);
      System.Console.WriteLine(SerializerResponse(inquiries[0]));
    }

    private string SerializerResponse(Inquiry inquiry)
    {
      Response response = new Response
      {
        BlockId = inquiry.BlockId,
        Availability = new(),
        Inquiries = new List<ResponseInquiry> {
          new() {
            Id = inquiry.Id,
            Response = "Accepted",
            AcceptedInterval = new() {
              Start = inquiry.Interval!.Start.ToUniversalTime(),
              End = inquiry.Interval!.End.ToUniversalTime()
            }
          }
        }
      };
      var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = new LowerCaseNamingPolicy()};
      string jsonString = JsonSerializer.Serialize(response, options);
      return jsonString;
    }

    private static List<Inquiry> DeserializeInquiries(string json)
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
        throw new InvalidDataException();
      }
    }

  }
}