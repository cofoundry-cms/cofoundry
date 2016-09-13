using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    public static class DangerousFileConstants
    {
        /// <summary>
        /// A collection of file extensions which could be considered dangerous and should not be
        /// transmitted via email or allowed to be downloaded directly from a webpage.
        /// </summary>
        /// <remarks>
        /// The list originates from the MS Outlook block list at http://office.microsoft.com/en-gb/outlook-help/blocked-attachments-in-outlook-HA001229952.aspx#BM3
        /// </remarks>
        public static readonly string[] DangerousFileExtensions = new string[] {
            ".ade",".adp",".app",".asp",".bas",".bat",".cer",".chm",".cmd",".cnt",".com",".cpl",".crt",".csh",".der",".exe",".fxp",".gadget",".hlp",".hpj",".hta",".inf",".ins",".isp",".its", ".js", ".jse", 
            ".ksh", ".lnk", ".mad", ".maf", ".mag", ".mam", ".maq", ".mar", ".mas", ".mat", ".mau", ".mav", ".maw", ".mda", ".mdb", ".mde", ".mdt", ".mdw", ".mdz", ".msc", ".msh",".msh1",".msh2",".mshxml",
            ".msh1xml",".msh2xml",".msi", ".msp", ".mst", ".ops", ".osd",".pcd", ".pif", ".plg",".prf", ".prg", ".pst", ".reg", ".scf", ".scr", ".sct", ".shb", ".shs", ".ps1",".ps1xml",".ps2",".ps2xml",
            ".psc1",".psc2",".tmp", ".url", ".vb", ".vbe", ".vbp",".vbs", ".vsmacros ",".vsw",".ws", ".wsc", ".wsf", ".wsh", ".xnk",
        };

        /// <summary>
        /// A collection of mime types which could be considered dangerous and should not be
        /// transmitted via email or allowed to be downloaded directly from a webpage.
        /// </summary>
        /// <remarks>
        /// The list originates here http://www.htmlhelpcentral.com/messageboard/showthread.php?17092-List-of-Dangerous-MIME-Types
        /// </remarks>
        public static readonly string[] DangerousMimeTypes = new string[] {
            "application/x-msdownload",
            "application/x-msdos-program",
            "application/x-msdos-windows",
            "application/x-download",
            "application/bat",
            "application/x-bat",
            "application/com",
            "application/x-com",
            "application/exe",
            "application/x-exe",
            "application/x-winexe",
            "application/x-winhlp",
            "application/x-winhelp",
            "application/x-javascript",
            "application/hta",
            "application/x-ms-shortcut",
            "application/octet-stream",
            "vms/exe",
            "text/javascript","text/scriptlet"
        };
    }
}
