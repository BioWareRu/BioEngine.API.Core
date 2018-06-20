using System.Collections.Generic;
using BioEngine.Core.API.Interfaces;

namespace BioEngine.Core.API.Response
{
    public class RestResponse
    {
        public RestResponse(int code, IEnumerable<IErrorInterface> errors = null)
        {
            Code = code;
            if (errors != null)
            {
                Errors = errors;
            }
        }

        public int Code { get; }

        public IEnumerable<IErrorInterface> Errors { get; protected set; }
    }
}