using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API.Request
{
    public class RequestParams
    {
        [FromQuery(Name = "offset")] public int Offset { get; set; }

        [FromQuery(Name = "limit")] public int Limit { get; set; } = 20;

        [FromQuery(Name = "order")] public string OrderBy { get; set; }

        public int PageSize => Limit;

        public int? Page => Offset > 0 ? Offset / Limit + 1 : 1;
    }
}