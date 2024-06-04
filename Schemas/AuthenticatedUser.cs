namespace MafiaAPI.Schemas
{
    public class AuthenticatedUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool matchInProgress { get; set; }
        public string matchInProgressId { get; set; }
    }
}
