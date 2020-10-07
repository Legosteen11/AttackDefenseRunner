using System;
using System.Text.RegularExpressions;
using AttackDefenseRunner.Model;

namespace AttackDefenseRunner.Util
{
    public static class TagHelper
    {
        private static bool IsValid(string tag) =>
            Regex.IsMatch(tag,".*:.*");

        public static string GetImage(string tag)
        {
            if (!IsValid(tag))
                throw new ArgumentException("Invalid Tag pattern");

            return tag.Split(":")[0];
        }
        
        public static string GetVersion(string tag)
        {
            if (!IsValid(tag))
                throw new ArgumentException("Invalid Tag pattern");

            return tag.Split(":")[1];
        }

        public static string CreateName(string tag, int imageId)
            => imageId + tag;

        public static string CreateImageOnlyName(string tag, int imageId)
            => imageId + GetImage(tag);
    }
}