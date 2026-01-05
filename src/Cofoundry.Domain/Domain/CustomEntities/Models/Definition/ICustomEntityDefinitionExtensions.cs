using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for custom entity definitions.
/// </summary>
public static class ICustomEntityDefinitionExtensions
{
    extension<TDefition>(TDefition definition) where TDefition : ICustomEntityDefinition
    {
        public Type GetDataModelType()
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
        public IReadOnlyDictionary<string, string> GetTerms()
        {
            if (definition is ICustomizedTermCustomEntityDefinition customEntityTermDefinition
                && customEntityTermDefinition.CustomTerms != null)
            {
                var terms = customEntityTermDefinition
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
