using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data
{
    [Serializable]
    public struct SerializableKeyValuePair<K, V>
    {
        public SerializableKeyValuePair(K key, V value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public K Key { get; set; }
        public V Value { get; set; }
    }
}
