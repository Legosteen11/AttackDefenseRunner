using System;
using System.Linq;
using AttackDefenseRunner.Hubs;
using AttackDefenseRunner.Model;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace AttackDefenseRunner.Util
{
    public class SettingsHelper
    {
        public const string FLAG_REGEX_KEY = "FLAG_REGEX";
        public const string VULNSERVERS_KEY = "VULNSERVERS";
        public const string ATTACKSERVERS_KEY = "ATTACKSERVERS";

        private readonly ADRContext _context;
        private readonly IHubContext<MonitorHub> _monitor;

        public SettingsHelper(ADRContext context, IHubContext<MonitorHub> monitor)
        {
            _context = context;
            _monitor = monitor;
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
            
            //Send to all clients
            _monitor.Clients.All.SendAsync("ConfigUpdate", key, value);
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