namespace MafiaAPI.Models
{
    public class PlayerState
    {
        public string Id { get; set; }
        public bool IsAlive { get; set; }
        public string? RoleId { get; set; }
        public Role? Role { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string MatchId { get; set; }
        public Match Match { get; set; }
    }
}
