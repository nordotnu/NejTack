using System.Text.Json;

namespace Utilities
{
  public class LowerCaseNamingPolicy : JsonNamingPolicy
  {
    public override string ConvertName(string name) =>
        name.ToLower();
  }

}