using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Settings used by FileSystemFileStoreService to determine where
    /// to store files.
    /// </summary>
    public class FileSystemFileStorageSettings : CofoundryConfigurationSettingsBase
    {
        public FileSystemFileStorageSettings()
        {
            FileRoot = "~/App_Data/Files/";
        }

        /// <summary>
        /// The directory root in which to store files such as images, documents and 
        /// file caches. The default value is "~/App_Data/Files/". IPathResolver is 
        /// used to resolve this path so by default you should be able to use application 
        /// relative and absolute file paths.
        /// </summary>
        public string FileRoot { get; set; }
    }
}
