namespace Calendar.Shared.Models.WebApi.Response
{
    public abstract class ResponseModelBase
    {
        public string ToJson()
            => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
