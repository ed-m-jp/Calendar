using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Calendar.Shared.Models.WebApi.Response
{
    public class PartialEventResponse : ResponseModelBase
    {

        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public int Id { get; }

        [JsonPropertyName("title")]
        [JsonProperty("title")]
        public string Title { get; }

        [JsonPropertyName("startTime")]
        [JsonProperty("startTime")]
        public DateTime StartTime { get; }

        [JsonPropertyName("endTime")]
        [JsonProperty("endTime")]
        public DateTime EndTime { get; }

        public PartialEventResponse(int id, string title, DateTime startTime, DateTime endTime)
        {
            Id = id;
            Title = title;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
