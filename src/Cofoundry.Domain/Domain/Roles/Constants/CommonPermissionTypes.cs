using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// These are common permissions types used by most entity types and are defined here
    /// to make them easy to re-use when defining IEntityPermission objects.
    /// </summary>
    public static class CommonPermissionTypes
    {
        public static readonly string ReadPermissionCode = "COMRED";

        /// <summary>
        /// Access to create a new entity
        /// </summary>
        /// <param name="entityNamePlural">The plural name of the entity, used in the description e.g. "Create new pages"</param>
        public static PermissionType Create(string entityNamePlural)
        {
            Condition.Requires(entityNamePlural).IsNotNullOrWhiteSpace();
            return new PermissionType("COMCRT", "Create", "Create new " + entityNamePlural.ToLower());
        }
        /// <summary>
        /// Basic read access to an new entity, required to do anything at all with 
        /// an entity
        /// </summary>
        /// <param name="entityNamePlural">The plural name of the entity, used in the description e.g. "Access pages"</param>
        public static PermissionType Read(string entityNamePlural)
        {
            Condition.Requires(entityNamePlural).IsNotNullOrWhiteSpace();
            return new PermissionType(ReadPermissionCode, "Read", "Access " + entityNamePlural.ToLower());
        }

        /// <summary>
        /// Access to the admin module for an entity
        /// </summary>
        /// <param name="entityNamePlural">The plural name of the entity, used in the description e.g. "View the pages module in the admin panel"</param>
        public static PermissionType AdminModule(string entityNamePlural)
        {
            Condition.Requires(entityNamePlural).IsNotNullOrWhiteSpace();
            return new PermissionType("COMMOD", "Admin Module", "View the " + entityNamePlural.ToLower() + " module in the admin panel");
        }

        /// <summary>
        /// Access to update an exiting entity
        /// </summary>
        /// <param name="entityNamePlural">The plural name of the entity, used in the description e.g. "Update new pages"</param>
        public static PermissionType Update(string entityNamePlural)
        {
            Condition.Requires(entityNamePlural).IsNotNullOrWhiteSpace();
            return new PermissionType("COMUPD", "Update", "Update " + entityNamePlural.ToLower());
        }

        /// <summary>
        /// Access to delete an exiting entity
        /// </summary>
        /// <param name="entityNamePlural">The plural name of the entity, used in the description e.g. "Delete pages"</param>
        public static PermissionType Delete(string entityNamePlural)
        {
            Condition.Requires(entityNamePlural).IsNotNullOrWhiteSpace();
            return new PermissionType("COMDEL", "Delete", "Delete " + entityNamePlural.ToLower());
        }

        /// <summary>
        /// General write access to an object. Use this as a more simple form of providing create/update/delete permissions
        /// instead of using more granular permission types.
        /// </summary>
        /// <param name="entityNamePlural">The plural name of the entity, used in the description e.g. "Add, update or delete pages"</param>
        public static PermissionType Write(string entityNamePlural)
        {
            Condition.Requires(entityNamePlural).IsNotNullOrWhiteSpace();
            return new PermissionType("COMWRT", "Write", "Add, update or delete " + entityNamePlural.ToLower());
        }
    }
}
