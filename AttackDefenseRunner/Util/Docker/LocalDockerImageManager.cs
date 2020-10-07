using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AttackDefenseRunner.Util.Docker
{
    public class LocalDockerImageManager : IDockerImageManager
    {
        private readonly ADRContext _context;
        private readonly DockerTagManager _tagManager;
        private readonly DockerClient _dockerClient;

        public LocalDockerImageManager(ADRContext context, DockerTagManager tagManager)
        {
            _context = context;
            _tagManager = tagManager;
            // TODO: Create something nicer for this so we can add settings etc.
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
        }

        public async Task<DockerContainer> StartContainer(DockerTag tag)
        {
            string containerName = TagHelper.CreateName(tag.Tag, tag.DockerImageId);
            
            // Start tag
            // Pull the image
            // TODO: Fix this with some API call because this is not secure :-).
            $"docker pull {tag.Tag}".Bash();
            
            // First create the image:
            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = tag.Tag,
                Name = containerName
            });
            
            Log.Information("Started container {name}, {id} for {tag}", containerName, container.ID, tag.Tag);
            
            DockerContainer dockerContainer = new DockerContainer
            {
                DockerTag = tag,
                DockerId = container.ID
            };

            await _context.AddAsync(dockerContainer);
            await _context.SaveChangesAsync();

            return dockerContainer;
        }

        public async Task StopContainer(string id)
        {
            await _dockerClient.Containers.StopContainerAsync(id, new ContainerStopParameters());
            try
            {
                await _dockerClient.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters());
            }
            catch (Exception)
            {
                // Ignore exception
            }

            // Remove any containers from the database with this id
            _context.RemoveRange(await _context.DockerContainers.Where(dockerContainer => dockerContainer.DockerId == id).ToListAsync());
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImage(string tagString)
        {
            // Get or create the tag
            DockerTag tag = await _tagManager.GetOrCreateTag(tagString);

            // Check if there are any running instances of this image
            var runningIds = await GetContainers(TagHelper.GetImage(tagString), TagHelper.CreateImageOnlyName(tagString, tag.DockerImageId));

            bool alreadyRunning = false;
            
            foreach (var container in runningIds)
            {
                if (container.Image != tagString)
                {
                    Log.Information("Stopping container {id}", container.ID);
                    await StopContainer(container.ID);
                }
                else
                    alreadyRunning = true;
            }

            if (!alreadyRunning)
            {
                await StartContainer(tag);
            }
        }

        public async Task StopImage(string tagString)
        {
            DockerImage image = await _tagManager.GetImage(tagString);
            
            if (image == null)
                throw new ImageNotFoundException(tagString);

            var runningIds = await GetContainers(TagHelper.GetImage(tagString), TagHelper.CreateImageOnlyName(tagString, image.Id));

            string imageString = TagHelper.GetImage(tagString);
            
            foreach (var container in runningIds)
            {
                // Somehow we found an incorrect match we should ignore
                if (container.Image.Split(":")[0] != imageString) 
                    continue;
                
                Log.Information("Stopping container {id}", container.ID);
                await StopContainer(container.ID);
            }
        }

        private async Task<ICollection<ContainerListResponse>> GetContainers(string imageString, string nameString)
            => (await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    // {"label", new Dictionary<string, bool> {{imageString, true}}}
                },
                All = true
            })).Where(container => TagHelper.GetImage(container.Image) == imageString).ToList();
    }
}