using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class AuthorizedTaskTypeDefinitionRepository : IAuthorizedTaskTypeDefinitionRepository
    {
        private readonly Dictionary<string, IAuthorizedTaskTypeDefinition> _definitions;

        public AuthorizedTaskTypeDefinitionRepository(
            IEnumerable<IAuthorizedTaskTypeDefinition> definitions
            )
        {
            DetectInvalidDefinitions(definitions);
            _definitions = definitions.ToDictionary(k => k.AuthorizedTaskTypeCode);

        }

        public IAuthorizedTaskTypeDefinition GetByCode(string authorizedTaskTypeCode)
        {
            var area = _definitions.GetOrDefault(authorizedTaskTypeCode);

            return area;
        }

        public IAuthorizedTaskTypeDefinition GetRequiredByCode(string authorizedTaskTypeCode)
        {
            var area = GetByCode(authorizedTaskTypeCode);

            if (area == null)
            {
                throw new EntityNotFoundException<IAuthorizedTaskTypeDefinition>(authorizedTaskTypeCode, $"Authorized task type '{authorizedTaskTypeCode}' is not registered. but has been requested.");
            }

            return area;
        }

        public IEnumerable<IAuthorizedTaskTypeDefinition> GetAll()
        {
            return _definitions.Select(a => a.Value);
        }

        private void DetectInvalidDefinitions(IEnumerable<IAuthorizedTaskTypeDefinition> definitions)
        {
            var nullCode = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.AuthorizedTaskTypeCode))
                .FirstOrDefault();

            if (nullCode != null)
            {
                var message = nullCode.GetType().Name + " does not have a definition code specified.";
                throw new InvalidAuthorizedTaskTypeDefinitionException(message, nullCode, definitions);
            }

            var duplicateCode = definitions
                .GroupBy(e => e.AuthorizedTaskTypeCode, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (duplicateCode != null)
            {
                var message = "Duplicate IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode: " + duplicateCode.Key;
                throw new InvalidAuthorizedTaskTypeDefinitionException(message, duplicateCode.First(), definitions);
            }

            var notValidCode = definitions
                .Where(d => !SqlCharValidator.IsValid(d.AuthorizedTaskTypeCode, 6))
                .FirstOrDefault();

            if (notValidCode != null)
            {
                var message = notValidCode.GetType().Name + " has an invalid AuthorizedTaskTypeCode. The code must be 6 characters in length and can include only non-unicode characters.";
                throw new InvalidAuthorizedTaskTypeDefinitionException(message, notValidCode, definitions);
            }

            var nullName = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.Name))
                .FirstOrDefault();

            if (nullName != null)
            {
                var message = nullName.GetType().Name + " does not have a name specified.";
                throw new InvalidAuthorizedTaskTypeDefinitionException(message, nullName, definitions);
            }

            var nameTooLong = definitions
                .Where(d => d.Name.Length > 20)
                .FirstOrDefault();

            if (nameTooLong != null)
            {
                var message = nameTooLong.GetType().Name + " has a name that is more than 20 characters in length. All authorization task type definition names must be 20 characters or less.";
                throw new InvalidAuthorizedTaskTypeDefinitionException(message, nameTooLong, definitions);
            }

            var duplicateName = definitions
                .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (duplicateName != null)
            {
                var message = "Duplicate IAuthorizedTaskTypeDefinition.Name: " + duplicateName.Key;
                throw new InvalidAuthorizedTaskTypeDefinitionException(message, duplicateName.First(), definitions);
            }
        }
    }
}
