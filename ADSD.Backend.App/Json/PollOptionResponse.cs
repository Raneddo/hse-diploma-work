using System.Text.Json.Serialization;

namespace ADSD.Backend.App.Json;

public class PollOptionResponse
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Count { get; set; }
    
    [JsonPropertyName("is_chosen")]
    public bool IsChosen { get; set; }
}