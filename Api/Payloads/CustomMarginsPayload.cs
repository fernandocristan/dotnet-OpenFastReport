namespace Api.Payloads
{
    public class CustomMarginsPayload
    {
        public int? BottomMilimiters { get; set; }
        public int? LeftMilimiters { get; set; }
        public int? RightMilimiters { get; set; }
        public int? TopMilimiters { get; set; } = 100;
    }
}