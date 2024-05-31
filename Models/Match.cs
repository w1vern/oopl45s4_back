namespace MafiaAPI.Models
{
    public class Match
    {
        public string Id { get; set; }
        public DateTime MatchStart { get; set; }
        public DateTime MatchEnd { get; set; }
        public string MatchResult { get; set; }
    }
}
