namespace Models
{
  public class ResponseInquiry
  {
    public int Id { get; set; }
    public string? Response { get; set; }
    public Interval? AcceptedInterval { get; set; }
  }
}