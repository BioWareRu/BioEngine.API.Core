using BioEngine.Core.API.Request;

namespace BioEngine.Core.API.Entities
{
    public class Page : SiteEntityRestModel<Core.Entities.Page, int>
    {
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public string Text { get; set; }
    }
}