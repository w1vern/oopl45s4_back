namespace MafiaAPI.Schemas
{
    public class RoleInfo
    {
        public string Id;
        public int Count;
    }
    public class StartMatchRequest
    {
        public List<RoleInfo> Roles;
    }
}
