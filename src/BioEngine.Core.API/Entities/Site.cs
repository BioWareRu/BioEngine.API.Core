using BioEngine.Core.API.Request;

namespace BioEngine.Core.API.Entities
{
    public class Site : RestModel<Core.Entities.Site, int>
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}