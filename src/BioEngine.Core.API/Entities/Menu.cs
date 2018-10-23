using System.Collections.Generic;
using BioEngine.Core.API.Request;
using BioEngine.Core.Entities;

namespace BioEngine.Core.API.Entities
{
    public class Menu : SiteEntityRestModel<int>
    {
        public string Title { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}