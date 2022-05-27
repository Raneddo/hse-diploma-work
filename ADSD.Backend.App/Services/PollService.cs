using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Exceptions;
using ADSD.Backend.App.Json;

namespace ADSD.Backend.App.Services;

public class PollService
{
    private readonly AppDbClient _dbClient;

    public PollService(AppDbClient dbClient)
    {
        _dbClient = dbClient;
    }

    public IEnumerable<PollResponse> GetPolls(int userId)
    {
        var polls = _dbClient.GetPolls();
        foreach (var poll in polls)
        {
            poll.Options = _dbClient.GetOptionsByPoll(poll.Id, userId).ToList();
            poll.VotersCount = poll.Options.Sum(x => x.Count);
            yield return poll;
        }
    }

    public PollResponse GetPollById(int pollId, int userId)
    {
        var poll = _dbClient.GetPollById(pollId);
        if (poll == null)
        {
            throw new NotFoundException("No poll for id");
        }
        poll.Options = _dbClient.GetOptionsByPoll(pollId, userId).ToList();
        poll.VotersCount = poll.Options.Sum(x => x.Count);
        return poll;
    }

    public int CreatePoll(string name, string text, List<PollOptionRequest> options, bool multiChoice)
    {
        var pollId = _dbClient.CreatePoll(name, text, multiChoice);
        foreach (var option in options)
        {
            _dbClient.AddOption(pollId, option.Text);
        }

        return pollId;
    }

    public void VotePollOptions(int pollId, int userId, List<int> options)
    {
        var poll = _dbClient.GetPollById(pollId);
        if (poll == null)
        {
            return;
        }
        if (!poll.MultiChoice && options.Count > 1)
        {
            options = options.GetRange(0, 1);
        }
        
        var pollOptions = _dbClient
            .GetOptionsByPoll(pollId, userId)
            .Select(x => x.Id)
            .Intersect(options)
            .ToList();
        foreach (var option in pollOptions)
        {
            _dbClient.UnvotePollOptions(userId, option);
            _dbClient.VotePollOptions(userId, option);
        }
    }
    
    public void UnvotePollOptions(int pollId, int userId)
    {
        var pollOptions = _dbClient
            .GetOptionsByPoll(pollId, userId)
            .Select(x => x.Id)
            .ToList();
        foreach (var option in pollOptions)
        {
            _dbClient.UnvotePollOptions(userId, option);
        }
    }

    public void DeletePoll(int id)
    {
        _dbClient.DeletePoll(id);
    }
}