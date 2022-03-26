namespace ADSD.Backend.App.Json;

public class UserBaseInfo
{
    public int Id { get; set; }
    public string Prefix { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Organization { get; set; }
    public string Email { get; set; }
    public string ApplicationStatus { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; }
}