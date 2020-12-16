using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class UserAreaDefinitionRepository : IUserAreaDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, IUserAreaDefinition> _userAreas;

        public UserAreaDefinitionRepository(
            IEnumerable<IUserAreaDefinition> userAreas
            )
        {
            DetectInvalidDefinitions(userAreas);
            _userAreas = userAreas.ToDictionary(k => k.UserAreaCode);
        }

        private void DetectInvalidDefinitions(IEnumerable<IUserAreaDefinition> definitions)
        {
            var nullName = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.UserAreaCode))
                .FirstOrDefault();

            if (nullName != null)
            {
                var message = nullName.GetType().Name + " does not have a definition code specified.";
                throw new InvalidUserAreaDefinitionException(message, nullName, definitions);
            }

            var duplicateCode = definitions
                .GroupBy(e => e.UserAreaCode)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (duplicateCode != null)
            {
                var message = "Duplicate IUserAreaDefinition.UserAreaCode: " + duplicateCode.Key;
                throw new InvalidUserAreaDefinitionException(message, duplicateCode.First(), definitions);
            }

            var nameNot3Chars = definitions
                .Where(d => d.UserAreaCode.Length != 3)
                .FirstOrDefault();

            if (nameNot3Chars != null)
            {
                var message = nameNot3Chars.GetType().Name + " has a definition code that is not 3 characters in length. All user area definition codes must be 3 characters.";
                throw new InvalidUserAreaDefinitionException(message, nameNot3Chars, definitions);
            }
        }

        #endregion

        /// <summary>
        /// Gets a user area definition using the unique code. Throws an exception
        /// if the user area is not registered.
        /// </summary>
        /// <param name="code">Uniquely identifying user area code.</param>
        public IUserAreaDefinition GetByCode(string code)
        {
            var area = _userAreas.GetOrDefault(code);

            if (area == null)
            {
                throw new EntityNotFoundException("UserArea not registered: " + code);
            }

            return area;
        }

        /// <summary>
        /// Returns all user areas defitions registered in the system.
        /// </summary>
        public IEnumerable<IUserAreaDefinition> GetAll()
        {
            return _userAreas.Select(a => a.Value);
        }
    }
}
