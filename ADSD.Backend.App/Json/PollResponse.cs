using System.Text.Json.Serialization;
using ADSD.Backend.App.Models;

namespace ADSD.Backend.App.Json;

public class PollResponse
{
   public int Id { get; set; }
   public string Name { get; set; }
   public string Text { get; set; }
   
   [JsonPropertyName("voters_count")]
   public int VotersCount { get; set; }
   
   [JsonPropertyName("multi_choice")]
   public bool MultiChoice { get; set; }
   
   public List<PollOptionResponse> Options { get; set; }
   
   public int? AgendaId { get; set; }
}