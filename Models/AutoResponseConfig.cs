namespace Models
{
  public class AutoResponseConfig
  {
    public bool Status { get; set; }
    public required int Cycle { get; set; }
    public required List<Interval> Accept { get; set; }
  }
}