namespace ADSD.Backend.App.Json;

public class CreatePollRequest
{
    public string Name { get; set; }
    public string Text { get; set; }
    public List<PollOptionRequest> Options { get; set; }
    public bool MultiChoice { get; set; }
}