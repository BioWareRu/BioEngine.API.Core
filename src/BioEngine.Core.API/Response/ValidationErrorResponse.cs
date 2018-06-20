using BioEngine.Core.API.Interfaces;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Response
{
    public class ValidationErrorResponse : IErrorInterface
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationErrorResponse(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}