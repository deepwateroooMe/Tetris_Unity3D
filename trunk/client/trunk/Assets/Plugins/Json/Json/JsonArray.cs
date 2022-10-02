﻿namespace System.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public class JsonArray : JsonValue, IList<JsonValue>, ICollection<JsonValue>, IEnumerable<JsonValue>, IEnumerable
    {
        private List<JsonValue> list;

        public JsonArray(params JsonValue[] items)
        {
            this.list = new List<JsonValue>();
            this.AddRange(items);
        }

        public JsonArray(IEnumerable<JsonValue> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            this.list = new List<JsonValue>(items);
        }

        public void Add(JsonValue item)
        {
            //if (item == null)
            //{
            //    throw new ArgumentNullException("item");
            //}
            this.list.Add(item);
        }

        public void AddRange(IEnumerable<JsonValue> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            this.list.AddRange(items);
        }

        public void AddRange(params JsonValue[] items)
        {
            if (items != null)
            {
                this.list.AddRange(items);
            }
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public bool Contains(JsonValue item)
        {
            return this.list.Contains(item);
        }

        public void CopyTo(JsonValue[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(JsonValue item)
        {
            return this.list.IndexOf(item);
        }

        public void Insert(int index, JsonValue item)
        {
            this.list.Insert(index, item);
        }

        public bool Remove(JsonValue item)
        {
            return this.list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            stream.WriteByte(0x5b);
            for (int i = 0; i < this.list.Count; i++)
            {
                this.list[i].Save(stream);
                if (i < (this.Count - 1))
                {
                    stream.WriteByte(0x2c);
                    stream.WriteByte(0x20);
                }
            }
            stream.WriteByte(0x5d);
        }

        IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public override int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public sealed override JsonValue this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                this.list[index] = value;
            }
        }

        public override System.Json.JsonType JsonType
        {
            get
            {
                return System.Json.JsonType.Array;
            }
        }
    }
}

