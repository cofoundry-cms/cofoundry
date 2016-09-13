using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PasswordCryptographyResult
    {
        public PasswordEncryptionVersion EncryptionVersion { get; set; }

        public string Hash { get; set; }
    }
}
