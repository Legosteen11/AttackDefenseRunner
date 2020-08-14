using System;

namespace AttackDefenseRunner.Util
{
    public class CouldNotFindSettingException : Exception
    {
        public string Key { get; }

        public CouldNotFindSettingException(string key) : base($"Could not find setting for {key}")
        {
        }
    }
}