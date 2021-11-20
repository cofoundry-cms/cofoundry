using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// Roles can be defined in code as well as in the admin panel. Defining
    /// a role in code means that it gets added automatically at startup. 
    /// Additionally we have a RoleCode that we can use to query 
    /// the role programatically.
    /// 
    /// See https://www.cofoundry.org/docs/framework/roles-and-permissions
    /// </summary>
    public class MemberRole : IRoleDefinition
    {
        public const string MemberRoleCode = "MEM";

        /// <summary>
        /// The role title is used to identify the role and select it in the admin 
        /// UI and therefore must be unique. Max 50 characters.
        /// </summary>
        public string Title { get { return "Member"; } }

        /// <summary>
        /// The role code is a unique three letter code that can be used to reference 
        /// the role programatically. The code must be unique and convention is to use 
        /// upper case, although code matching is case insensitive.
        /// </summary>
        public string RoleCode { get { return MemberRoleCode; } }

        /// <summary>
        /// A role must be assigned to a user area, in this case the role is used for members
        /// </summary
        public string UserAreaCode { get { return MemberUserArea.Code; } }
    }
}
