using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class ICustomEntityDefinitionExtensions
    {
        public static Type GetDataModelType<TDefition>(this TDefition definition)
            where TDefition : ICustomEntityDefinition
        {
            var dataModelType = definition
                .GetType()
                .GetInterfaces()
                .Select(t => t.GetTypeInfo())
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICustomEntityDefinition<>))
                .Select(i => i.GetGenericArguments().Single())
                .SingleOrDefault();

            if (dataModelType == null)
            {
                var message = definition + " does not inherit from ICustomEntityDefinition<TDataModel>. Do not inherit from ICustomEntityDefinition directly, but instead use the generic version.";
                throw new InvalidCustomEntityDefinitionException(message, definition);
            }

            return dataModelType;
        }

        /// <summary>
        /// Gets the combined term descriptions for the custom entity, merging
        /// any custom terms with the defaults.
        /// </summary>
        public static Dictionary<string, string> GetTerms<TDefition>(this TDefition definition)
            where TDefition : ICustomEntityDefinition
        {
            var customEntityTermDefinition = definition as ICustomizedTermCustomEntityDefinition;
            if (customEntityTermDefinition != null && customEntityTermDefinition.CustomTerms != null)
            {
                var terms = ((ICustomizedTermCustomEntityDefinition)definition)
                    .CustomTerms
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                foreach (var defaultTerm in CustomizableCustomEntityTermKeys.Defaults)
                {
                    if (!terms.ContainsKey(defaultTerm.Key))
                    {
                        terms.Add(defaultTerm.Key, defaultTerm.Value);
                    }
                }

                return terms;
            }
            else
            {
                return CustomizableCustomEntityTermKeys.Defaults;
            }
        }
    }
}
