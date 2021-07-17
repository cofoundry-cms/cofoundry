using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Dummy user class to provide to the Microsoft.AspNetCore.Identity.IPasswordHasher 
    /// used by the PasswordCryptographyService. There's no implementation as it's not 
    /// required by the defaulty hasher.
    /// </summary>
    public class PasswordHasherUser
    {
    }
}
