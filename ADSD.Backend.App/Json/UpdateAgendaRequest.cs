namespace ADSD.Backend.App.Json;

public class UpdateAgendaRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<int> Speakers { get; set; }
    public List<int> Polls { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool Active { get; set; }
}