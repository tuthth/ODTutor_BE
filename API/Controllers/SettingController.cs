using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        [HttpGet("all")]
        public async Task<string[]> Directories()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Tìm đường dẫn gốc của dự án (project root)
            string projectRootPath = baseDirectory.Replace("\\bin\\Debug\\net8.0\\", "\\");
            return Directory.GetDirectories(projectRootPath);
        }
        [HttpGet("base")]
        public async Task<string> BaseDirectory() => AppDomain.CurrentDomain.BaseDirectory;
        [HttpGet("current")]
        public async Task<string> DynamicDirectory() => Directory.GetCurrentDirectory();
        
    }
}
