namespace MafiaAPI.Models
{
    public class PlayerState
    {
        public string Id { get; set; }
        public bool IsAlive { get; set; }
        public string? RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string MatchId { get; set; }
        public virtual Match Match { get; set; }
    }
}
