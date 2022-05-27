namespace ADSD.Backend.App.Json;

public class ChatGroupCreateRequest
{
    public string Name { get; set; }
    public int[] UserIds { get; set; }
}