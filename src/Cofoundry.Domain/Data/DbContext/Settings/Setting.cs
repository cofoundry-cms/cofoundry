using System;

namespace Cofoundry.Domain.Data
{
    public class Setting
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}