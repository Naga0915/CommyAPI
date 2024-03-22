using CommyAPI.Models.Util;
using CommyAPI.Setting.Media;
using Microsoft.AspNetCore.Mvc;

namespace CommyAPI.Controllers.Util
{
    [ApiController]
    [Route("/api/icon")]
    public class IconController : Controller
    {
        [HttpGet("g/{id}")]
        public async Task<FileResult> General(string id)
        {
            var stream = await Icon.GetIconAsync(id, CommyServices.BBS);
            return File(stream, "image/png");
        }

        [HttpGet("t/{id}")]
        public async Task<FileResult> Trip(string id)
        {
            var stream = await Icon.GetIconAsync(id, CommyServices.BBSTrip);
            return File(stream, MIME.mimeBBSIcon);
        }

    }
}
