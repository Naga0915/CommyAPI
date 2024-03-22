using Microsoft.AspNetCore.Mvc;

namespace CommyAPI.Controllers.BBS
{
    [ApiController]
    [Route("/api/bbs")]
    public class BBSController : Controller
    {
        [HttpGet("{room}")]
        public async Task<IActionResult> GetThread(string room, int from, int num)
        {
            return Ok();
        }
    }
}
