namespace System.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public abstract class JsonValue : IEnumerable
    {
        protected JsonValue()
        {
        }

        public virtual bool ContainsKey(string key)
        {
            throw new InvalidOperationException();
        }

        private string DoEscapeString(StringBuilder sb, string src, int cur)
        {
            int start = cur;
            for (int i = cur; i < src.Length; i++)
            {
                if ((src[i] == '"') || (src[i] == '\\'))
                {
                    sb.Append(src, start, i - start);
//                    string temp1 = sb.ToString();
                    sb.Append('\\');
                    if (src.Length > i + 1 && src[i + 1] != '"')
                    {
                        sb.Append(src[i++]);
                    }
                    else
                    {
                      
                    }
                    start = i;
                }
            }
            sb.Append(src, start, src.Length - start);
            return sb.ToString();
        }

        internal string EscapeString(string src)
        {
            if (src == null)
            {
                return null;
            }
            for (int i = 0; i < src.Length; i++)
            {
                if ((src[i] == '"') || (src[i] == '\\'))
                {
                    StringBuilder sb = new StringBuilder();
                    if (i > 0)
                    {
                        sb.Append(src, 0, i);
                    }
                    return this.DoEscapeString(sb, src, i);
                }
            }
            return src;
        }

       
        public static JsonValue Load(TextReader textReader)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException("textReader");
            }
            return ToJsonValue(new JavaScriptObjectDeserializer(textReader.ReadToEnd(), true).BasicDeserialize());
        }

        public static implicit operator JsonValue(bool value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(byte value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(char value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(DateTime value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(DateTimeOffset value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(decimal value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(double value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(Guid value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(short value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(int value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(long value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator bool(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToBoolean(((JsonPrimitive) value).Value);
        }

        public static implicit operator byte(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToByte(((JsonPrimitive) value).Value);
        }

        public static implicit operator char(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToChar(((JsonPrimitive) value).Value);
        }

        public static implicit operator DateTime(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (DateTime) ((JsonPrimitive) value).Value;
        }

        public static implicit operator DateTimeOffset(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (DateTimeOffset) ((JsonPrimitive) value).Value;
        }

        public static implicit operator decimal(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToDecimal(((JsonPrimitive) value).Value);
        }

        public static implicit operator double(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToDouble(((JsonPrimitive) value).Value);
        }

        public static implicit operator Guid(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (Guid) ((JsonPrimitive) value).Value;
        }

        public static implicit operator short(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToInt16(((JsonPrimitive) value).Value);
        }

        public static implicit operator int(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToInt32(((JsonPrimitive) value).Value);
        }

        public static implicit operator long(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToInt64(((JsonPrimitive) value).Value);
        }

        public static implicit operator sbyte(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToSByte(((JsonPrimitive) value).Value);
        }

        public static implicit operator float(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToSingle(((JsonPrimitive) value).Value);
        }

        public static implicit operator string(JsonValue value)
        {
            if (value == null)
            {
                return null;
            }
            return (string) ((JsonPrimitive) value).Value;
        }

        public static implicit operator TimeSpan(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (TimeSpan) ((JsonPrimitive) value).Value;
        }

        public static implicit operator ushort(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToUInt16(((JsonPrimitive) value).Value);
        }

        public static implicit operator uint(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToUInt16(((JsonPrimitive) value).Value);
        }

        public static implicit operator ulong(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Convert.ToUInt64(((JsonPrimitive) value).Value);
        }

        public static implicit operator Uri(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (Uri) ((JsonPrimitive) value).Value;
        }

        public static implicit operator JsonValue(sbyte value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(float value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(string value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(TimeSpan value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(ushort value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(uint value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(ulong value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(Uri value)
        {
            return new JsonPrimitive(value);
        }

        public static JsonValue Parse(string jsonString)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException("jsonString");
            }
            return Load(new StringReader(jsonString));
        }

        public virtual void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.Save(new StreamWriter(stream));
        }

        public virtual void Save(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }
            this.SaveInternal(textWriter);
        }

        private void SaveInternal(TextWriter w)
        {
            bool following;
            switch (this.JsonType)
            {
                case System.Json.JsonType.String:
                    w.Write('"');
                    w.Write(this.EscapeString(((JsonPrimitive) this).GetFormattedString()));
                    w.Write('"');
                    return;

                case System.Json.JsonType.Object:
                    w.Write('{');
                    following = false;
                    foreach (KeyValuePair<string, JsonValue> pair in (JsonObject) this)
                    {
                        if (following)
                        {
                            w.Write(",");
                        }
                        w.Write('"');
                        w.Write(this.EscapeString(pair.Key));
                        w.Write("\":");
                        if (pair.Value == null)
                        {
                            w.Write("null");
                        }
                        else
                        {
                            pair.Value.SaveInternal(w);
                        }
                        following = true;
                    }
                    w.Write('}');
                    return;

                case System.Json.JsonType.Array:
                    w.Write('[');
                    following = false;
                    foreach (JsonValue v in (IEnumerable<JsonValue>) ((JsonArray) this))
                    {
                        if (following)
                        {
                            w.Write(",");
                        }
                        if (v == null)
                        {
                            w.Write("null");
                        }
                        else
                        {
                            v.SaveInternal(w);
                        }
                        following = true;
                    }
                    w.Write(']');
                    return;

                case System.Json.JsonType.Boolean:
                    //w.Write((this != null) ? "true" : "false");
                    w.Write(((JsonPrimitive)this).GetFormattedString());
                    return;
            }
            w.Write(((JsonPrimitive) this).GetFormattedString());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException();
        }

        private static IEnumerable<KeyValuePair<string, JsonValue>> ToJsonPairEnumerable(IEnumerable<KeyValuePair<string, object>> kvpc)
        {
            foreach (KeyValuePair<string, object> iteratorVariable0 in kvpc)
            {
                yield return new KeyValuePair<string, JsonValue>(iteratorVariable0.Key, ToJsonValue(iteratorVariable0.Value));
            }
        }

        private static JsonValue ToJsonValue(object ret)
        {
            if (ret == null)
            {
                return null;
            }
            IEnumerable<KeyValuePair<string, object>> kvpc = ret as IEnumerable<KeyValuePair<string, object>>;
            if (kvpc != null)
            {
                return new JsonObject(ToJsonPairEnumerable(kvpc));
            }
            IEnumerable<object> arr = ret as IEnumerable<object>;
            if (arr != null)
            {
                return new JsonArray(ToJsonValueEnumerable(arr));
            }
            if (ret is bool)
            {
                return new JsonPrimitive((bool) ret);
            }
            if (ret is byte)
            {
                return new JsonPrimitive((byte) ret);
            }
            if (ret is char)
            {
                return new JsonPrimitive((char) ret);
            }
            if (ret is decimal)
            {
                return new JsonPrimitive((decimal) ret);
            }
            if (ret is double)
            {
                return new JsonPrimitive((double) ret);
            }
            if (ret is float)
            {
                return new JsonPrimitive((float) ret);
            }
            if (ret is int)
            {
                return new JsonPrimitive((int) ret);
            }
            if (ret is long)
            {
                return new JsonPrimitive((long) ret);
            }
            if (ret is sbyte)
            {
                return new JsonPrimitive((sbyte) ret);
            }
            if (ret is short)
            {
                return new JsonPrimitive((short) ret);
            }
            if (ret is string)
            {
                return new JsonPrimitive((string) ret);
            }
            if (ret is uint)
            {
                return new JsonPrimitive((uint) ret);
            }
            if (ret is ulong)
            {
                return new JsonPrimitive((ulong) ret);
            }
            if (ret is ushort)
            {
                return new JsonPrimitive((ushort) ret);
            }
            if (ret is DateTime)
            {
                return new JsonPrimitive((DateTime) ret);
            }
            if (ret is DateTimeOffset)
            {
                return new JsonPrimitive((DateTimeOffset) ret);
            }
            if (ret is Guid)
            {
                return new JsonPrimitive((Guid) ret);
            }
            if (ret is TimeSpan)
            {
                return new JsonPrimitive((TimeSpan) ret);
            }
            if (!(ret is Uri))
            {
                throw new NotSupportedException(string.Format("Unexpected parser return type: {0}", ret.GetType()));
            }
            return new JsonPrimitive((Uri) ret);
        }

        private static IEnumerable<JsonValue> ToJsonValueEnumerable(IEnumerable<object> arr)
        {
            foreach (object iteratorVariable0 in arr)
            {
                yield return ToJsonValue(iteratorVariable0);
            }
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            this.Save(sw);
            return sw.ToString();
        }

        public virtual int Count
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public virtual JsonValue this[int index]
        {
            get
            {
                throw new InvalidOperationException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public virtual JsonValue this[string key]
        {
            get
            {
                throw new InvalidOperationException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public abstract System.Json.JsonType JsonType { get; }
    }
}

