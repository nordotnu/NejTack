namespace Models
{
    public class Inquiry
    {
        public int Id { get; set; }
        public int BlockId { get; set; }
        public DateTime Day { get; set; }
        public Interval? Interval { get; set; }
        public double Hours { get; set; }
        public List<string>? Information { get; set; }
        public List<DetailedInformation>? DetailedInformation { get; set; }
        public List<DetailedInformation>? AssignedInformation { get; set; }
        public string? Response { get; set; }
        public string? ResponseName { get; set; }
        public Interval? AcceptedInterval { get; set; }
        public string? Availability { get; set; }
        public List<Interval>? AvailableIntervals { get; set; }
        public int? ShiftId { get; set; }
        public string? StatusName { get; set; }
        public string? StatusColor { get; set; }
        public bool Opened { get; set; }
        public bool Open { get; set; }
        public bool AssignmentUnconfirmedByEmployee { get; set; }
        public bool ResponseOpen { get; set; }
    }
}