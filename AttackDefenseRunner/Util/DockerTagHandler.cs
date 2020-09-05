using System;
using System.Collections.Generic;
using System.Linq;
using AttackDefenseRunner.Model;
using Serilog;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AttackDefenseRunner.Util
{
    public class DockerTagHandler
    {
        private readonly ADRContext _context;
        private const string DockerRegex = "^(?:(?=[^:\\/]{1,253})(?!-)[a-zA-Z0-9-]{1,63}(?<!-)(?:\\.(?!-)[a-zA-Z0-9-]{1,63}(?<!-))*(?::[0-9]{1,5})?/)?(?<name>(?![._-])(?:[a-z0-9._-]*)(?<![._-])(?:/(?![._-])[a-z0-9._-]*(?<![._-]))*):(?<version>(?![.-])[a-zA-Z0-9_.-]{1,128})?$";
        public DockerTagHandler(ADRContext context)
        {
            _context = context;
        }

        public async Task AddDockerTag(string inputDockerTag)
        {
            Match match = Regex.Match(inputDockerTag, DockerRegex);
            if (match.Success)
            {
                Log.Information("Docker tag matched!");
                string newDockerName = match.Groups["name"].Value;
                string newDockerVersion = match.Groups["version"].Value;
                string newDockerTag = match.Value;
                Log.Information("Docker tag: {DockerTag}", newDockerTag);
                Log.Information("Docker tag name: {DockerName}", newDockerName);
                Log.Information("Docker tag version: {DockerVersion}", newDockerVersion);


                DockerImage di = GetOrCreateDockerImage(newDockerName);
                if (GetDockerTag(newDockerTag) == null)
                {
                    //Create new dockertag in database and ask servers to run it
                    await RegisterDockerTag(newDockerVersion, newDockerTag, di);
                }
            }
            else
            {
                Log.Information("Docker tag did not match...");
                List<DockerTag> dockerlist = this.FetchTagsFromDatabase();
                Log.Information("Docker tag list: {@dockerlist}",dockerlist);
            }
            

        }

        private List<DockerContainer> FetchContainersFromDatabase() =>
            _context.DockerContainers
                .Include(t => t.DockerTag)
                .Include(t => t.DockerTag.DockerImage)
                .ToList();

        private List<DockerTag> FetchTagsFromDatabase() =>
            _context.DockerTags
                .Include(t => t.DockerImage)
                .ToList();
        
        private DockerImage GetOrCreateDockerImage(string dockerName)
        {
            DockerImage dockerImage = _context
                .DockerImages
                .FirstOrDefault(i => i.Name == dockerName);
            if (dockerImage == null)
            {
                dockerImage = new DockerImage
                {
                    Name = dockerName
                };
                _context.Add(dockerImage);
                _context.SaveChanges();
            }
            
            return dockerImage;
        }

        private DockerTag GetDockerTag(string dockerTag) =>
            _context.DockerTags.FirstOrDefault(t => t.Tag == dockerTag);
        

        /// <summary>
        /// Create the DockerTag in the database and run it on remote servers
        /// </summary>
        /// <param name="inputDockerVersion"></param>
        /// <param name="inputDockerTag"></param>
        /// <param name="inputDockerImage"></param>
        /// <returns></returns>
        private async Task RegisterDockerTag(string inputDockerVersion, string inputDockerTag, DockerImage inputDockerImage)
        {
            DockerTag dockerTag = new DockerTag
            {
                Version = inputDockerVersion,
                Tag = inputDockerTag,
                DockerImage = inputDockerImage,
                Timestamp = DateTime.Now
            };
            _context.Add(dockerTag);
            await _context.SaveChangesAsync();
            
            //TODO: add server shit
        }
        
    }
}