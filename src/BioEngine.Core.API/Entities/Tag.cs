using BioEngine.Core.API.Request;

namespace BioEngine.Core.API.Entities
{
    public class Tag : RestModel<int>
    {
        public string Name { get; set; }
    }
}