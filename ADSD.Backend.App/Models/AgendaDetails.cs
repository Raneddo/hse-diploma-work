namespace ADSD.Backend.App.Models;

public class AgendaDetails
{
    public AgendaDetails(AgendaHeader agendaHeader, List<Speaker> speakers)
    {
        AgendaHeader = agendaHeader;
        Speakers = speakers;
    }

    public AgendaHeader AgendaHeader { get; }
    public List<Speaker> Speakers { get; }
}