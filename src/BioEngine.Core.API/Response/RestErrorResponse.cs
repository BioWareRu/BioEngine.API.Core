using BioEngine.Core.API.Interfaces;

namespace BioEngine.Core.API.Response
{
    public class RestErrorResponse : IErrorInterface
    {
        public RestErrorResponse(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}