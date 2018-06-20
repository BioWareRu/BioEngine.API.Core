using System.Collections.Generic;
using BioEngine.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Response
{
    public class ListResponse<T, TPkType> : RestResponse where T : IEntity<TPkType>
    {
        public ListResponse(IEnumerable<T> data, int totalitem) : base(StatusCodes.Status200OK)
        {
            Data = data;
            TotalItems = totalitem;
        }

        [JsonProperty] public IEnumerable<T> Data { get; }

        [JsonProperty] public int TotalItems { get; }
    }
}