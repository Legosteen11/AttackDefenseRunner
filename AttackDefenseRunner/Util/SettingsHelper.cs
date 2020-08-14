using System;
using System.Linq;
using AttackDefenseRunner.Model;
using Serilog;

namespace AttackDefenseRunner.Util
{
    public class SettingsHelper
    {
        public const string FLAG_REGEX_KEY = "FLAG_REGEX";
        public const string VULNSERVERS_KEY = "VULNSERVERS";
        public const string ATTACKSERVERS_KEY = "ATTACKSERVERS";
        
        private ADRContext _context { get; }

        public SettingsHelper(ADRContext context)
        {
            _context = context;
        }

        public void SetValue(string key, string value)
        {
            GlobalSetting setting = _context.GlobalSettings.SingleOrDefault(s => s.Key == key);
            
            // Check if it exists
            if (setting != null)
            {
                Log.Information("Updating value for {key} to {value} in GlobalSettings", key, value);
                setting.Value = value;
            }
            else
            {
                Log.Information("Inserting new GlobalSetting for {key} with {value}", key, value);
                setting = new GlobalSetting
                {
                    Key = key,
                    Value = value
                };
                _context.GlobalSettings.Add(setting);
            }

            _context.SaveChanges();
        }

        public string GetValue(string key)
        {
            try
            {
                return _context.GlobalSettings.First(s => s.Key == key).Value;
            }
            catch (Exception)
            {
                // Return empty string on exception
                return "";
            }
        }
        
    }
}