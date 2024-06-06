namespace MafiaAPI.Schemas
{
    public class PlayersRoleRequest
    {
        public string PlayerId { get; set; }
        public string? RoleName { get; set; }
        public bool IsAlive {get; set;}
    }
}
