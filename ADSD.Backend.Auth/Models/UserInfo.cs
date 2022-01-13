namespace ADSD.Backend.Auth.Models
{
    public record UserInfo
    {
        public long? UserId { get; init; }
        
        public string Email { get; init; }
        
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Prefix { get; init; }
        
        public string DocumentName { get; init; }
        public string DocumentNumber { get; init; }
        public string Groups { get; set; }
    }
}