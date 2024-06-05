namespace MafiaAPI.Models
{
    public class Match
    {
        public string Id { get; set; }
        public int currentState { get; set; }
        public DateTime? MatchStart { get; set; }
        public DateTime? MatchEnd { get; set; }
        public string? MatchResult { get; set; }
        public virtual List<PlayerState> PlayerStates { get; set; } = new();
    }
}
