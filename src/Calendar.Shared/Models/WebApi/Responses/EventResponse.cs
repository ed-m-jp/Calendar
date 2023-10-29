using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calendar.Shared.Models.WebApi.Response
{
    public class EventResponse : ResponseModelBase
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public int Id { get; }

        [JsonPropertyName("title")]
        [JsonProperty("title")]
        public string Title { get; }

        [JsonPropertyName("description")]
        [JsonProperty("description")]
        public string Description { get; }

        [JsonPropertyName("allDay")]
        [JsonProperty("allDay")]
        public bool AllDay { get; }

        [JsonPropertyName("startTime")]
        [JsonProperty("startTime")]
        public DateTime StartTime { get; }

        [JsonPropertyName("endTime")]
        [JsonProperty("endTime")]
        public DateTime EndTime { get; }

        public EventResponse(int id, string title, string description, bool allDay, DateTime startTime, DateTime endTime)
        {
            Id = id;
            Title = title;
            Description = description;
            AllDay = allDay;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
