namespace ADSD.Backend.App.Json;

public class UpdateAgendaRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<int> Speakers { get; set; }
    public List<Session> Sessions { get; set; }
    public bool Active { get; set; }
}