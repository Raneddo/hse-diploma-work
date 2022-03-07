using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Models;

namespace ADSD.Backend.App.Services;

public class AgendaService
{
    private readonly AppDbClient _appDbClient;

    public AgendaService(AppDbClient appDbClient)
    {
        _appDbClient = appDbClient;
    }

    public IEnumerable<AgendaHeader> GetAgendasList(int count = int.MaxValue, int offset = 0)
    {
        return _appDbClient.GetAgendasList(count, offset);
    }

    public AgendaDetails GetAgendaInfo(int id)
    {
        var header =  _appDbClient.GetAgendaInfo(id);

        if (header == null)
        {
            return null;
        }

        var speakers = _appDbClient.GetSpeakersByAgenda(header.Id).ToList();

        var details = new AgendaDetails(header, speakers);

        return details;
    }


    public int AddAgenda(AgendaHeader header)
    {
       return _appDbClient.AddAgenda(header);
    }

    public void UpdateAgenda(int id, UpdateAgendaRequest updateAgendaRequest)
    {
        _appDbClient.UpdateAgenda(id, updateAgendaRequest.Title, updateAgendaRequest.Description, updateAgendaRequest.Active);
        _appDbClient.UpdateAgendaSessions(id, updateAgendaRequest.Sessions);
        _appDbClient.UpdateSpeakers(id, updateAgendaRequest.Speakers);
    }
}