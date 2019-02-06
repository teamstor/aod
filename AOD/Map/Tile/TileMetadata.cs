using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD
{
    /// <summary>
    /// Tile attributes helper.
    /// </summary>
    public class TileMetadata : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> _values = new Dictionary<string, string>();
        private SortedSet<string> _orderedNames = new SortedSet<string>();

        /// <summary>
        /// If any metadata values are set.
        /// </summary>
        public bool HasValuesSet
        {
            get
            {
                return _values.Count > 0;
            }
        }

        /// <summary>
        /// Amount of keys set.
        /// </summary>
        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        public string this[string key]
        {
            get
            {
                if(!_values.ContainsKey(key))
                    return "";
                return _values[key];
            }
            set
            {
                if(_values.ContainsKey(key) && string.IsNullOrEmpty(value))
                {
                    _values.Remove(key);
                    _orderedNames.Remove(key);
                }
                else
                {
                    if(!_values.ContainsKey(key))
                    {
                        _values.Add(key, value);
                        _orderedNames.Add(key);
                    }
                    else _values[key] = value;
                }
            }
        }

        public bool IsKeySet(string key)
        {
            return _values.ContainsKey(key);
        }

        public string GetOrDefault(string key, string defaultValue)
        {
            string v = defaultValue;
            _values.TryGetValue(key, out v);

            if(v == "")
                return defaultValue;
            return v;
        }

        public override int GetHashCode()
        {
            if(!HasValuesSet)
                return 0;

            int hashcode = 0xFFFF;
            foreach(string s in _orderedNames)
                hashcode ^= s.GetHashCode() ^ this[s].GetHashCode();

            return hashcode;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)_values).GetEnumerator();
        }
    }
}
