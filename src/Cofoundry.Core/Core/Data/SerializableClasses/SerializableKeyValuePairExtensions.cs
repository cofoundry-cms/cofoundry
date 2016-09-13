using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data
{
    public static class SerializableKeyValuePairExtensions
    {
        public static void Add<K,V>(this IList<SerializableKeyValuePair<K,V>> source, K key, V value)
        {
            source.Add(new SerializableKeyValuePair<K,V>(key,value));
        }
    }
}
