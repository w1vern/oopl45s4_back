namespace MafiaAPI.Schemas
{
    public class RoleInfo
    {
        public string Id { get; set; }
        public int Count { get; set; }
    }
    public class StartMatchRequest
    {
        public List<RoleInfo> Roles = [];
    }
}
