namespace Models
{
  public class Response
  {
    public int BlockId { get; set; }
    public object? MessageFromEmployee { get; set; }
    public List<object?>? Availability { get; set; }
    public List<ResponseInquiry>? Inquiries { get; set; }
  }
}