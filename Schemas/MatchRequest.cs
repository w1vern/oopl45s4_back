namespace MafiaAPI.RequestModels
{
    public class MatchRequest
    {
        public string Id { get; set; }
        public DateTime? MatchStart { get; set; }
        public DateTime? MatchEnd { get; set; }
        public string? MatchResult { get; set; }
        public List<string> PlayersIds { get; set; } = new();
    }
}
