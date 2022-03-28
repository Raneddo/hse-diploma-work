using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Json;

namespace ADSD.Backend.App.Services;

public class AgendaService
{
    private readonly AppDbClient _appDbClient;

    public AgendaService(AppDbClient appDbClient)
    {
        _appDbClient = appDbClient;
    }

    public IEnumerable<AgendaResponse> GetAgendasList(int count = int.MaxValue, int offset = 0)
    {
        var agendas = _appDbClient.GetAgendasList(count, offset).ToList();
        foreach (var agenda in agendas)
        {
            agenda.Speakers = _appDbClient
                .GetSpeakersByAgenda(agenda.Id)
                .Select(x => x.Id)
                .ToList();
            agenda.Polls = _appDbClient
                .GetPollsByAgenda(agenda.Id)
                .ToList();
        }

        return agendas;
    }

    public AgendaResponse GetAgendaInfo(int id)
    {
        var agenda = _appDbClient.GetAgendaInfo(id);
        agenda.Polls = _appDbClient.GetPollsByAgenda(agenda.Id).ToList();
        agenda.Speakers = _appDbClient
            .GetSpeakersByAgenda(agenda.Id)
            .Select(x => x.Id)
            .ToList();
        return agenda;
    }

    public int CreateAgenda(UpdateAgendaRequest request)
    {
       var agendaId = _appDbClient.CreateAgenda(request);

       if (request.Speakers != null)
       {
           _appDbClient.UpdateSpeakers(agendaId, request.Speakers);
       }
       
       if (request.Polls != null)
       {
           foreach (var pollId in request.Polls)
           {
               _appDbClient.SetPollAgenda(agendaId, pollId);
           }
       }
       
       return agendaId;
    }

    public void UpdateAgenda(int id, UpdateAgendaRequest updateAgendaRequest)
    {
        _appDbClient.UpdateAgenda(id,
            updateAgendaRequest.Title,
            updateAgendaRequest.Description,
            updateAgendaRequest.StartDate,
            updateAgendaRequest.EndDate,
            updateAgendaRequest.Active);
        _appDbClient.UpdateSpeakers(id, updateAgendaRequest.Speakers);
    }

    public void DeleteAgenda(int id)
    {
        _appDbClient.DeleteAgenda(id);
    }
}