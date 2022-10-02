namespace System.Json
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class JsonPrimitive : JsonValue,IConvertible
    {
        private static readonly byte[] false_bytes = Encoding.UTF8.GetBytes("false");
        private static readonly byte[] true_bytes = Encoding.UTF8.GetBytes("true");
        private object value;

        public JsonPrimitive(bool value)
        {
            this.value = value;
        }

        public JsonPrimitive(byte value)
        {
            this.value = value;
        }

        public JsonPrimitive(char value)
        {
            this.value = value;
        }

        public JsonPrimitive(DateTime value)
        {
            this.value = value;
        }

        public JsonPrimitive(DateTimeOffset value)
        {
            this.value = value;
        }

        public JsonPrimitive(decimal value)
        {
            this.value = value;
        }

        public JsonPrimitive(double value)
        {
            this.value = value;
        }

        public JsonPrimitive(Guid value)
        {
            this.value = value;
        }

        public JsonPrimitive(short value)
        {
            this.value = value;
        }

        public JsonPrimitive(int value)
        {
            this.value = value;
        }

        public JsonPrimitive(long value)
        {
            this.value = value;
        }

        public JsonPrimitive(sbyte value)
        {
            this.value = value;
        }

        public JsonPrimitive(float value)
        {
            this.value = value;
        }

        public JsonPrimitive(string value)
        {
            this.value = value;
        }

        public JsonPrimitive(TimeSpan value)
        {
            this.value = value;
        }

        public JsonPrimitive(ushort value)
        {
            this.value = value;
        }

        public JsonPrimitive(uint value)
        {
            this.value = value;
        }

        public JsonPrimitive(ulong value)
        {
            this.value = value;
        }

        public JsonPrimitive(Uri value)
        {
            this.value = value;
        }

        internal string GetFormattedString()
        {
            switch (this.JsonType)
            {
                case System.Json.JsonType.String:
                    if (!(this.value is string) && (this.value != null))
                    {
                        if (this.value is DateTime)
                        {
                            return ((DateTime)this.value).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            return this.value.ToString();
                        }
                        //throw new NotImplementedException("GetFormattedString from value type " + this.value.GetType());
                    }
                    var result=(string)this.value;   
                    return result;

                case System.Json.JsonType.Number:
                    return ((IFormattable) this.value).ToString("G", NumberFormatInfo.InvariantInfo);
                case System.Json.JsonType.Boolean:
                    return (bool)this.value?"true":"false";
            }
            throw new InvalidOperationException();
        }

        public override void Save(Stream stream)
        {
            byte[] bytes;
            switch (this.JsonType)
            {
                case System.Json.JsonType.String:
                    stream.WriteByte(0x22);
                    bytes = Encoding.UTF8.GetBytes(base.EscapeString(this.value.ToString()));
                    stream.Write(bytes, 0, bytes.Length);
                    stream.WriteByte(0x22);
                    return;

                case System.Json.JsonType.Boolean:
                    if ((bool) this.value)
                    {
                        stream.Write(true_bytes, 0, 4);
                        return;
                    }
                    stream.Write(false_bytes, 0, 5);
                    return;
            }
            bytes = Encoding.UTF8.GetBytes(this.GetFormattedString());
            stream.Write(bytes, 0, bytes.Length);
        }

        public override System.Json.JsonType JsonType
        {
            get
            {
                if (this.value == null)
                {
                    return System.Json.JsonType.String;
                }
                switch (Type.GetTypeCode(this.value.GetType()))
                {
                    case TypeCode.Object:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.String:
                        return System.Json.JsonType.String;

                    case TypeCode.Boolean:
                        return System.Json.JsonType.Boolean;
                }
                return System.Json.JsonType.Number;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
        }

		#region IConvertible implementation
		public TypeCode GetTypeCode ()
		{
			return TypeCode.Object;
		}

		public bool ToBoolean (IFormatProvider provider)
		{
			return (bool)this.value;
		}

		public byte ToByte (IFormatProvider provider)
		{
			return (byte)this.value;
		}

		public char ToChar (IFormatProvider provider)
		{
			return (char)this.value;
		}

		public DateTime ToDateTime (IFormatProvider provider)
		{
			return (DateTime)this.value;
		}

		public decimal ToDecimal (IFormatProvider provider)
		{
			return (decimal)this.value;
		}

		public double ToDouble (IFormatProvider provider)
		{
			return (double)this.value;
		}

		public short ToInt16 (IFormatProvider provider)
		{
			return (short)this.value;
		}

		public int ToInt32 (IFormatProvider provider)
		{
			return (int)this.value;
		}

		public long ToInt64 (IFormatProvider provider)
		{
			if(value is int)
			{
				return (long)(int)value;
			}
			return (long)this.value;
		}

		public sbyte ToSByte (IFormatProvider provider)
		{
			return (sbyte)this.value;
		}

        public float ToSingle(IFormatProvider provider)
        {
            if (value is decimal)
            {
                return (float)(decimal)value;
            }
            else if (value is double)
            {
                return (float)(double)value;
            }
            else if (value is int)
            {
                return (float)(int)value;
            }
            return (float)this.value;
        }

		public string ToString (IFormatProvider provider)
		{
			return (string)this.value;
		}

        public override string ToString()
        {
            return  this.value.ToString();
        }

        public object ToType (Type conversionType, IFormatProvider provider)
		{
			return this.value;
		}

		public ushort ToUInt16 (IFormatProvider provider)
		{
			return (ushort)this.value;
		}

		public uint ToUInt32 (IFormatProvider provider)
		{
			return (uint)this.value;
		}

		public ulong ToUInt64 (IFormatProvider provider)
		{
			return (ulong)this.value;
		}
		#endregion
    }
}

