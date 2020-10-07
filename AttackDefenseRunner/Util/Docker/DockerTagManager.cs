using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using Microsoft.EntityFrameworkCore;

namespace AttackDefenseRunner.Util.Docker
{
    public class DockerTagManager
    {
        private readonly ADRContext _context;

        public DockerTagManager(ADRContext context)
        {
            _context = context;
        }
        
        public async Task<DockerImage> GetImage(string tagString)
            => await _context.DockerImages.Where(dockerImage => dockerImage.Name == TagHelper.GetImage(tagString)).FirstOrDefaultAsync();

        public async Task<DockerImage> GetOrCreateImage(string tagString)
        {
            string imageString = TagHelper.GetImage(tagString);

            // First check if the image exists in the database
            DockerImage image = await _context.DockerImages.Where(dockerImage => dockerImage.Name == imageString).FirstOrDefaultAsync();

            if (image != null) return image;
            
            // Image does not exist, we should make one
            image = new DockerImage
            {
                Name = imageString
            };

            await _context.AddAsync(image);
            await _context.SaveChangesAsync();

            return image;
        }

        public async Task<DockerTag> GetOrCreateTag(string tagString)
        {
            string versionString = TagHelper.GetVersion(tagString);

            DockerTag tag = await _context.DockerTags
                .Where(dockerTag => dockerTag.Tag == tagString).FirstOrDefaultAsync();

            if (tag != null) return tag;
            
            DockerImage image = await GetOrCreateImage(tagString);
            
            // Tag does not exist, we should make one
            tag = new DockerTag
            {
                Tag = tagString,
                Version = versionString,
                DockerImage = image
            };

            await _context.AddAsync(tag);
            await _context.SaveChangesAsync();

            return tag;
        }

        public async Task<ICollection<DockerContainer>> GetContainers()
            => await _context.DockerContainers.Include(container => container.DockerTag).ToListAsync();
    }
}