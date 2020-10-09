using System;

namespace AttackDefenseRunner.Util.Docker
{
    public class ImageNotFoundException : Exception
    {
        private readonly String Image;
        
        public ImageNotFoundException(string image) : base($"Image {image} not found")
        {
            Image = image;
        }
    }
}