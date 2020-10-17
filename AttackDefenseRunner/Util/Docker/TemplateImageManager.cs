using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Config;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AttackDefenseRunner.Util.Docker
{
    public class TemplateImageManager
    {
        private readonly ADRContext _context;
        private readonly DockerTagManager _tagManager;
        private readonly FileStrings _fileStrings;
        private readonly DockerClient _dockerClient;

        public TemplateImageManager(ADRContext context, DockerTagManager tagManager, IOptions<FileStrings> fileStrings)
        {
            _context = context;
            _tagManager = tagManager;
            _fileStrings = fileStrings.Value;
            // TODO: Create something nicer for this so we can add settings etc.
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
        }

        public async Task<ICollection<TemplateImage>> GetTemplates()
            => await _context.TemplateImages.ToListAsync();

        public async Task<IDictionary<string, string>> GetFiles()
        {
            // TODO: Read files from that directory and return them
            
            throw new NotImplementedException();
        }

        public async Task<DockerTag> CreateNewTag(DockerImage image, IDictionary<string, string> fileContents)
        {
            var tag = await _tagManager.GetOrCreateTag(image.Name + ":X");

            string version = tag.Id.ToString();
            tag.Version = version;
            tag.Tag = image.Name + ":" + version;

            await _context.SaveChangesAsync();

            var directory = GetFolder(tag);

            Directory.CreateDirectory(directory);

            foreach (var (filename, content) in fileContents)
            {
                await using var file = File.CreateText(filename);

                await file.WriteAsync(content);
            }

            // Build the image
            await BuildDockerImage(tag);

            return tag;
        }

        private string GetFolder(DockerTag tag)
            => _fileStrings.CustomImagesDir + "/" + TagHelper.GetImage(tag.Tag) + ":" + tag.Id;

        private async Task BuildDockerImage(DockerTag tag)
        {
            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromSrc = GetFolder(tag),
                Tag = tag.Tag
            }, null, null, CancellationToken.None);
        }

        public async Task<DockerImage> CreateNewImage(TemplateImage template, string name)
        {
            var imageName = TagHelper.GetImage(name);
            var directory = _fileStrings.CustomImagesDir + "/" + imageName;

            // Create directory
            Directory.CreateDirectory(_fileStrings.CustomImagesDir);
            Directory.CreateDirectory(directory);

            return await _tagManager.GetOrCreateImage(imageName);
        }
    }
}