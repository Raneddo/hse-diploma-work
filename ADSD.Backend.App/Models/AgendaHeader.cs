namespace ADSD.Backend.App.Models;

public class AgendaHeader
{
    public AgendaHeader(int id, string title, string description, bool isActive)
    {
        Id = id;
        Title = title;
        Description = description;
        IsActive = isActive;
    }

    public int Id { get; }
    public string Title { get; }
    public string Description { get; }
    public bool IsActive { get; }
    
}