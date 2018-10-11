using BioEngine.Core.API.Request;

namespace BioEngine.Core.API.Entities
{
    public class Site : RestModel<int>
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}