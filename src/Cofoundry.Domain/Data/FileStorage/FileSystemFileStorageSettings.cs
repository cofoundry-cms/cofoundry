using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain.Data
{
    public class FileSystemFileStorageSettings : CofoundryConfigurationSettingsBase
    {
        public FileSystemFileStorageSettings()
        {
            FileRoot = "~/App_Data/Files/";
        }

        public string FileRoot { get; set; }
    }
}
