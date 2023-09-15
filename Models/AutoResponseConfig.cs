namespace Models
{
  public class AutoResponseConfig
  {
    public bool Status { get; set; }
    public bool RejectOthers { get; set; }
    public required List<Interval> Accept { get; set; }
  }
}