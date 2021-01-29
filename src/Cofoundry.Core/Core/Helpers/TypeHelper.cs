using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Helper for working with Type information.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Attempts to get the type argument for a generic collection type 
        /// or array e.g. string in IEnumerable&lt;string&gt; or string[]. 
        /// If the specified type isn't a singler collection type then null 
        /// is returned.
        /// </summary>
        public static Type GetCollectionTypeOrNull(Type collectionType)
        {
            if (collectionType == null) return null;

            if (!typeof(IEnumerable).IsAssignableFrom(collectionType))
            {
                return null;
            }

            Type singularType = null;
            if (collectionType.IsGenericType)
            {
                var genericParameters = collectionType.GetGenericArguments();

                if (genericParameters?.Length != 1)
                {
                    return null;
                }

                singularType = genericParameters.Single();
            }
            else if (collectionType.IsArray)
            {
                singularType = collectionType.GetElementType();
            }

            return singularType;
        }
    }
}
