namespace MafiaAPI.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PlayerState> PlayerStates { get; set; } = [];
    }
}
