using System.Text.Json.Serialization;

namespace ADSD.Backend.App.Json;

public class AgendaResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<int> Speakers { get; set; }
    public List<int> Polls { get; set; }
    
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; set; }
    
    [JsonPropertyName("end_date")]
    public DateTimeOffset? EndDate { get; set; }
}