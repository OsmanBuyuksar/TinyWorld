using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    [System.Serializable]
    public class SerializableDict<K, V>
    {
        [SerializeField]
        public List<K> keys;
        [SerializeField]
        public List<V> values;

        public SerializableDict()
        {
            keys = new List<K>();
            values = new List<V>();
        }

        public void AddElement(K key, V value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public void SetElement(K key, V value)
        {
            if (keys.IndexOf(key) == -1)
                throw new System.NullReferenceException("There is not a matching key element in the keys list");

            values[keys.IndexOf(key)] = value;
        }

        public void AddElements(K[] keys, V[] values)
        {
            if (keys.Length != values.Length)
                throw new System.RankException("Given lists should be same length");

            this.keys.AddRange(keys);
            this.values.AddRange(values);
        }

        public V GetValue(K key)
        {
            if (keys.IndexOf(key) == -1)
                throw new System.NullReferenceException("There is not a matching key element in the keys list");

            return values[keys.IndexOf(key)];
        }

        public V this[K key]
        {
            get { return GetValue(key); }
            set { SetElement(key, value); }
        }
    }
}
