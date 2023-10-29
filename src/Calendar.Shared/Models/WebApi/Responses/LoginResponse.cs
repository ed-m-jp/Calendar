using Calendar.Shared.Models.WebApi.Response;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Calendar.Shared.Models.WebApi.Responses
{
    public class LoginResponse: ResponseModelBase
    {
        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string Username { get; }

        [JsonPropertyName("token")]
        [JsonProperty("token")]
        public string Token { get; }

        public LoginResponse(string username, string token)
        {
            Username = username;
            Token = token;
        }
    }
}
