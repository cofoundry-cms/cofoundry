using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Helper cvlass for cloning objects
    /// </summary>
    public static class ObjectCloner
    {
        private static ConcurrentDictionary<Tuple<Type, Type>, Delegate> _cachedIL = new ConcurrentDictionary<Tuple<Type, Type>, Delegate>();
        private static readonly MethodInfo _generateShallowCopyILMethod = typeof(ObjectCloner).GetMethod("GenerateShallowCopyIL", BindingFlags.NonPublic | BindingFlags.Static);

        #region public members

        /// <summary>
        /// Clones an object as a new instance of an implemented type.
        /// </summary>
        /// <typeparam name="TFrom">Type to clone from</typeparam>
        /// <typeparam name="TTo">Type to clone as (must be type that obj implements)</typeparam>
        /// <param name="obj">Object to clone</param>
        /// <returns>Cloned instance TTo</returns>
        public static TTo ShallowCopyAsImplementedType<TFrom, TTo>(TFrom obj) where TFrom : TTo
        {
            if (obj == null) return default(TTo);

            var fromType = typeof(TFrom);
            var toType = typeof(TTo);

            var cloneFunc = _cachedIL.GetOrAdd(new Tuple<Type, Type>(fromType, toType), (t) => GenerateShallowCopyIL<TFrom, TTo>(t));
            return ((Func<TFrom, TTo>)cloneFunc)(obj);
        }

        /// <summary>
        /// Clones an object as a new object of an implemented type.
        /// </summary>
        /// <remarks>
        /// Adapted from http://stackoverflow.com/a/966466/716689
        /// </remarks>
        /// <param name="obj">Object to clone</param>
        /// <param name="toType">Type to clone as (must be type that obj implements)</param>
        /// <returns>New cloned object</returns>
        public static object ShallowCopyAsImplementedType<TFrom>(TFrom obj, Type toType)
        {
            if (obj == null) return null;

            var objType = obj.GetType();
            var fromType = obj.GetType();

            var cloneFunc = _cachedIL.GetOrAdd(new Tuple<Type, Type>(fromType, toType), (t) => ExecuteGenerateShallowCopyIL(t));
            return cloneFunc.DynamicInvoke(obj);
        }

        #endregion

        #region private helpers

        private static Delegate ExecuteGenerateShallowCopyIL(Tuple<Type, Type> types)
        {
            return (Delegate)_generateShallowCopyILMethod
                     .MakeGenericMethod(types.Item1, types.Item2)
                     .Invoke(null, new object[] { types });
        }

        private static Delegate GenerateShallowCopyIL<TFrom, TTo>(Tuple<Type, Type> types)
        {
            var toType = typeof(TTo);
            var fromType = typeof(TFrom);

            if (toType == fromType)
            {
                throw new ArgumentException("Cannot clone: toType and fromType are the same type.");
            }
            if (!toType.IsAssignableFrom(fromType))
            {
                throw new ArgumentException("Cannot clone: toType does not inherit from  fromType.");
            }

            var dymMethod = new DynamicMethod("DoClone", toType, new Type[] { fromType }, true);
            var constructorInfo = toType.GetConstructor(new Type[] { });

            var generator = dymMethod.GetILGenerator();

            var lbf = generator.DeclareLocal(toType);

            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Stloc_0);

            foreach (var field in toType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // Load the new object on the eval stack... (currently 1 item on eval stack)
                generator.Emit(OpCodes.Ldloc_0);
                // Load initial object (parameter)          (currently 2 items on eval stack)
                generator.Emit(OpCodes.Ldarg_0);
                // Replace value by field value             (still currently 2 items on eval stack)
                generator.Emit(OpCodes.Ldfld, field);
                // Store the value of the top on the eval stack into the object underneath that value on the value stack.
                //  (0 items on eval stack)
                generator.Emit(OpCodes.Stfld, field);
            }

            // Load new constructed obj on eval stack -> 1 item on stack
            generator.Emit(OpCodes.Ldloc_0);
            // Return constructed object.   --> 0 items on stack
            generator.Emit(OpCodes.Ret);

            return dymMethod.CreateDelegate(typeof(Func<TFrom, TTo>));
        }

        #endregion
    }
}
