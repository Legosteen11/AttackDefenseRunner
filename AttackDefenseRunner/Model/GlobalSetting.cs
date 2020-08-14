using System.ComponentModel.DataAnnotations;

namespace AttackDefenseRunner.Model
{
    public class GlobalSetting
    {
        [Key]
        public string Key { get; set; }
        
        public string Value { get; set; }
    }
}