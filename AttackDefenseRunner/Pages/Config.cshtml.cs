﻿using AttackDefenseRunner.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class ConfigModel : PageModel
    {
        private readonly SettingsHelper _settings;
        private readonly ServiceManager _manager;

        public ConfigModel(SettingsHelper settings, ServiceManager manager)
        {
            _settings = settings;
            _manager = manager;
        }


        [BindProperty]
        public string FlagRegex { get; set; }
        
        [BindProperty]
        public string VulnServers { get; set; }
        
        [BindProperty]
        public string AttackServers { get; set; }

        public void OnPostConfig()
        {
            var flagregex = FlagRegex;
            var vulnservers = VulnServers;
            var attackservers = AttackServers;
            
            Log.Information("Flag regex is {flagregex}", flagregex);
            Log.Information("Vulnerable servers are {vulnservers}", vulnservers);
            Log.Information("Attacking servers are {attackservers}", attackservers);

            _settings.SetValue(SettingsHelper.FLAG_REGEX_KEY, flagregex);
            // TODO: Add the vulnservers to the database
            _settings.SetValue(SettingsHelper.VULNSERVERS_KEY, vulnservers);
            // TODO: Add the attackservers to the database
            _settings.SetValue(SettingsHelper.ATTACKSERVERS_KEY, attackservers);
        }
        
        public void OnPostStartService()
        {
            _manager.StartService();
        }
        
        public void OnPostStopService()
        {
            _manager.StopService();
        }
    }
}