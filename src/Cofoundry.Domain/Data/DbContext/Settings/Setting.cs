using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Setting
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
