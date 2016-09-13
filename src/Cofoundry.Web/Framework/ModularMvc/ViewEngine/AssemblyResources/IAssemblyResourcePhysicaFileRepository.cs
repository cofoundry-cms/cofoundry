using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Used to get the physical file represented by an AssemblyVirtualFile. Used
    /// in dev when the BypassEmbeddedContent setting is set to true to load the 
    /// files from disk instead of from the assembly. Allows file modification without re-compiling
    /// all the time.
    /// </summary>
    public interface IAssemblyResourcePhysicaFileRepository
    {
        Stream Open(AssemblyVirtualFileLocation location);
        string GetPhysicalFilePath(AssemblyVirtualFileLocation location);
        DateTime GetModifiedDate(AssemblyVirtualFileLocation location);
    }
}
