namespace Miku.Database.UserEntities
{
    public class UserPrefix
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public User AttachedUser { get; set; }
        public string Prefix { get; set; }
        public bool AllowDefaultPrefix { get; set; }
    }
}