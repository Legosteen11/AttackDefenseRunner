using System;

namespace AttackDefenseRunner.Util.Parsing
{
    public class ParseFailedException : Exception
    {
        public ParseFailedException()
            : base("Failed to parse json")
        {
        }
    }
}