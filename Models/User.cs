namespace MafiaAPI.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public virtual List<PlayerState> PlayerStates { get; set; } = new();
    }
}
