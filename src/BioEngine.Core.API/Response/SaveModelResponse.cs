using JetBrains.Annotations;

namespace BioEngine.Core.API.Response
{
    public class SaveModelResponse<T> : RestResponse
    {
        public SaveModelResponse(int code, T model) : base(code)
        {
            Model = model;
        }

        [UsedImplicitly] public T Model { get; }
    }
}