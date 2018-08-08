using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// As well as being able to create roles in the UI, roles can also
    /// be defined in code by implementing IRoleDefinition. To defined the
    /// permissions associated with the role implement a class that inherits 
    /// from IRoleInitializer
    /// </summary>
    public interface IRoleDefinition
    {
        /// <summary>
        /// The role title is used to identify the role and select it in the admin 
        /// UI and therefore must be unique. Max 50 characters.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The role code is a unique three letter code that can be used to reference the 
        /// role programatically. The code must be unique and convention is to use upper 
        /// case, although code matching is case insensitive.
        /// </summary>
        string RoleCode { get; }

        /// <summary>
        /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
        /// </summary>
        string UserAreaCode { get; }
    }
}
