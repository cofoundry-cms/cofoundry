using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to look up custom entity display model types from a string type 
    /// name. Used specifically when extracting the custom entity type from a
    /// razor view template.
    /// </summary>
    public class PageTemplateCustomEntityTypeMapper : IPageTemplateCustomEntityTypeMapper
    {
        private readonly IEnumerable<ICustomEntityDisplayModel> _customEntityDisplayModels;

        public PageTemplateCustomEntityTypeMapper(
            IEnumerable<ICustomEntityDisplayModel> customEntityDisplayModels
            )
        {
            _customEntityDisplayModels = customEntityDisplayModels;
        }

        /// <summary>
        /// Takes string type name and attempts to map it to a type that
        /// implements ICustomEntityDisplayModel. If one is found it is returned
        /// otherwise null is returned.
        /// </summary>
        /// <param name="typeName">
        /// Type name to look for. This is case sensitive and the namespace can 
        /// be included (but isn't checked).
        /// </param>
        /// <returns>ICustomEntityDisplayModel type if a match is found; otherwise null.</returns>
        public virtual Type Map(string typeName)
        {
            typeName = RemoveNamespace(typeName);
            if (string.IsNullOrEmpty(typeName)) return null;

            var displayModels = _customEntityDisplayModels.Where(m => m.GetType().Name == typeName);

            Debug.Assert(displayModels.Count() < 2, "Incorrect number of ICustomEntityDisplayModels registered with the name '" + typeName + "'. Expected 1, got " + displayModels.Count());

            Type result = null;

            if (displayModels.Any())
            {
                result = displayModels.First().GetType();
            }

            return result;
        }

        protected string RemoveNamespace(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            var dotIndex = typeName.LastIndexOf('.');
            if (dotIndex != -1 && dotIndex < typeName.Length)
            {
                typeName = typeName.Substring(dotIndex + 1);
            }

            return typeName;
        }
    }
}
