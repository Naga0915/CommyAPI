using CommyAPI.Models.Util;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

namespace CommyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        //[HttpGet("{id}")]
        //public async Task<FileResult> Get(string id)
        //{
        //    var stream = await Icon.GetIconAsync(id);
        //    return File(stream, "image/png");
        //}
    }
}
