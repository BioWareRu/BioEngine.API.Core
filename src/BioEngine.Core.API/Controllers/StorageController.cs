using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Storage;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.API.Controllers
{
    public class StorageController : ApiController
    {
        public StorageController(BaseControllerContext context) : base(context)
        {
        }

        [HttpGet("directories")]
        public async Task<IEnumerable<string>> ListDirectoriesAsync(string path)
        {
            return await Storage.ListDirectoriesAsync(path);
        }

        [HttpGet("items")]
        public async Task<IEnumerable<StorageItem>> ListItemsAsync(string path)
        {
            return await Storage.ListItemsAsync(path);
        }

        [HttpPost("items")]
        public async Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name, [FromQuery] string path)
        {
            var file = await GetBodyAsFileAsync();
            return await Storage.SaveFileAsync(file, name, $"{path}");
        }

        [HttpPost("directories")]
        public async Task<bool> CreateDirectoryAsync([FromQuery] string path)
        {
            try
            {
                await Storage.CreateDirectoryAsync(path);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.ToString());
                return false;
            }
        }
    }
}