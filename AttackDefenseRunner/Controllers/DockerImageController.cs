using System.Collections.Generic;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Docker;
using AttackDefenseRunner.Util.Parsing;
using AttackDefenseRunner.Util.Parsing.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AttackDefenseRunner.Controllers
{
    [Route("api/image/[controller]")]
    [ApiController]
    public class DockerImageController : Controller
    {
        private readonly IDockerImageManager _imageManager;
        private readonly ADRContext _context;
        private readonly DockerImageJsonParser _parser;

        public DockerImageController(IDockerImageManager imageManager, DockerImageJsonParser parser, ADRContext context)
        {
            _imageManager = imageManager;
            _parser = parser;
            _context = context;
        }

        [HttpGet("container")]
        public async Task<List<DockerContainer>> GetContainers()
            => await _context.DockerContainers.ToListAsync();

        [HttpPost("update")]
        public async Task<DockerContainer> UpdateImage()
        {
            DockerTagJson dockerTagJson;
            
            try
            {
                dockerTagJson = await _parser.ParseDockerJsonTag(Request.Body);
            }
            catch (ParseFailedException e)
            {
                Log.Error("JSON Parse failed {e}", e);
                return null;
            }
            
            return await _imageManager.UpdateImage(dockerTagJson.Tag);
        }
    }
}