namespace MafiaAPI.Models
{
    public class MatchUser
    {
        public string Id { get; set; }
        public Match Match { get; set; }
        public User User { get; set; }
    }
}
