using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;


namespace System.Json
{
    public class JsonObject : JsonValue, IDictionary<string, JsonValue>, ICollection<KeyValuePair<string, JsonValue>>, IEnumerable<KeyValuePair<string, JsonValue>>, IEnumerable
    {
        private Dictionary<string, JsonValue> map;

        public JsonObject(params KeyValuePair<string, JsonValue>[] items)
        {
            this.map = new Dictionary<string, JsonValue>();
            if (items != null)
            {
                this.AddRange(items);
            }
        }

        public JsonObject(IEnumerable<KeyValuePair<string, JsonValue>> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            this.map = new Dictionary<string, JsonValue>();
            this.AddRange(items);
        }

        public void Add(KeyValuePair<string, JsonValue> pair)
        {
            this.Add(pair.Key, pair.Value);
        }

        public void Add(string key, JsonValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.map.Add(key, value);
        }

        public void AddRange(params KeyValuePair<string, JsonValue>[] items)
        {
            this.AddRange((IEnumerable<KeyValuePair<string, JsonValue>>) items);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (KeyValuePair<string, JsonValue> pair in items)
            {
                this.map.Add(pair.Key, pair.Value);
            }
        }

        public void Clear()
        {
            this.map.Clear();
        }

        public override bool ContainsKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return this.map.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, JsonValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");

            }
            if (array.Rank != 1)
            {
                throw new ArgumentNullException("Arg_RankMultiDimNotSupported");
            }
            if (array.GetLowerBound(0) != 0)
            {
                 throw new ArgumentNullException("Arg_NonZeroLowerBound");
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException("index","ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
            }
            int count = this.map.Count;
            for (int i = 0; i < count; i++)
            {
                var keyVluePer = this.map.ElementAt(i);
                if (keyVluePer.GetHashCode() > 0)
                {
                    array[arrayIndex++] = keyVluePer;
                }
            }

        }

        public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            return this.map.GetEnumerator();
        }

        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return this.map.Remove(key);
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            stream.WriteByte(0x7b);
            foreach (KeyValuePair<string, JsonValue> pair in this.map)
            {
                stream.WriteByte(0x22);
                byte[] bytes = Encoding.UTF8.GetBytes(base.EscapeString(pair.Key));
                stream.Write(bytes, 0, bytes.Length);
                stream.WriteByte(0x22);
                stream.WriteByte(0x2c);
                stream.WriteByte(0x20);
                pair.Value.Save(stream);
            }
            stream.WriteByte(0x7d);
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item)
        {
            return this.map.ContainsKey(item.Key);
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item)
        {
            return this.map.Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.map.GetEnumerator();
        }

        public bool TryGetValue(string key, out JsonValue value)
        {
            return this.map.TryGetValue(key, out value);
        }

        public override int Count
        {
            get
            {
                return this.map.Count;
            }
        }

        public sealed override JsonValue this[string key]
        {
            get
            {
                return this.map[key];
            }
            set
            {
                this.map[key] = value;
            }
        }

        public override System.Json.JsonType JsonType
        {
            get
            {
                return System.Json.JsonType.Object;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.map.Keys;
            }
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<JsonValue> Values
        {
            get
            {
                return this.map.Values;
            }
        }
    }
}

