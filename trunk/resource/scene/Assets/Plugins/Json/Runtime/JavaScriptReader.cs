namespace System.Runtime.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal class JavaScriptReader
    {
        private int column;
        private bool has_peek;
        private int line = 1;
        private int peek;
        private bool prev_lf;
        private TextReader r;
//        private bool raise_on_number_error;
        private StringBuilder vb = new StringBuilder();

        public JavaScriptReader(TextReader reader, bool raiseOnNumberError)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this.r = reader;
//            this.raise_on_number_error = raiseOnNumberError;
        }

        private void Expect(char expected)
        {
            int c;
            if ((c = this.ReadChar()) != expected)
            {
                throw this.JsonError(string.Format("Expected '{0}', got '{1}'", expected, (char) c));
            }
        }

        private void Expect(string expected)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                if (this.ReadChar() != expected[i])
                {
                    throw this.JsonError(string.Format("Expected '{0}', differed at {1}", expected, i));
                }
            }
        }

        private Exception JsonError(string msg)
        {
            return new ArgumentException(string.Format("{0}. At line {1}, column {2}", msg, this.line, this.column));
        }

        private int PeekChar()
        {
            if (!this.has_peek)
            {
                this.peek = this.r.Read();
                this.has_peek = true;
            }
            return this.peek;
        }

        public object Read()
        {
            object v = this.ReadCore();
            this.SkipSpaces();
            if (this.r.Read() >= 0)
            {
                throw this.JsonError(string.Format("extra characters in JSON input", new object[0]));
            }
            return v;
        }

        private int ReadChar()
        {
            int v = this.has_peek ? this.peek : this.r.Read();
            this.has_peek = false;
            if (this.prev_lf)
            {
                this.line++;
                this.column = 0;
                this.prev_lf = false;
            }
            if (v == 10)
            {
                this.prev_lf = true;
            }
            this.column++;
            return v;
        }

        private object ReadCore()
        {
            this.SkipSpaces();
            int c = this.PeekChar();
            if (c < 0)
            {
                throw this.JsonError("Incomplete JSON input");
            }
		
            switch (c)
            {
                case 110:
                    this.Expect("null");
                    return null;

                case 0x74:
                    this.Expect("true");
                    return true;

                case 0x7b:
                {
                    this.ReadChar();
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    this.SkipSpaces();
                    if (this.PeekChar() == 0x7d)
                    {
                        this.ReadChar();
                        return obj;
                    }
                    do
                    {
                        this.SkipSpaces();
                        string name = this.ReadStringLiteral();
                        this.SkipSpaces();
                        this.Expect(':');
                        this.SkipSpaces();
                        obj[name] = this.ReadCore();
                        this.SkipSpaces();
                        c = this.ReadChar();
                    }
                    while ((c == 0x2c) || (c != 0x7d));
				    KeyValuePair<string, object>[] re=new KeyValuePair<string, object>[obj.Count];
					int i=0;
			        foreach(var l in obj)
					{
						re[i++]=l;	
					}
                    return re;
                }
                case 0x22:
                    return this.ReadStringLiteral();

                case 0x5b:
                {
                    this.ReadChar();
                    List<object> list = new List<object>();
                    this.SkipSpaces();
                    if (this.PeekChar() == 0x5d)
                    {
                        this.ReadChar();
                        return list;
                    }
                Label_0086:
                    list.Add(this.ReadCore());
                    this.SkipSpaces();
                    if (this.PeekChar() == 0x2c)
                    {
                        this.ReadChar();
                        goto Label_0086;
                    }
                    if (this.ReadChar() != 0x5d)
                    {
                        throw this.JsonError("JSON array must end with ']'");
                    }
                    return list.ToArray();
                }
                case 0x66:
                    this.Expect("false");
                    return false;
            }
            if (((0x30 > c) || (c > 0x39)) && (c != 0x2d))
            {
				//Logger.Error.Write("Unexpected character '{0}'",(byte)c);
                throw this.JsonError(string.Format("Unexpected character '{0}'", (char) c));
            }
            return this.ReadNumericLiteral();
        }

        private object ReadNumericLiteral()
        {
            int c;
            int exp;
            bool negative = false;
            if (this.PeekChar() == 0x2d)
            {
                negative = true;
                this.ReadChar();
                if (this.PeekChar() < 0)
                {
                    throw this.JsonError("Invalid JSON numeric literal; extra negation");
                }
            }
            decimal val = 0M;
            int x = 0;
            bool zeroStart = this.PeekChar() == 0x30;
            while (true)
            {
                c = this.PeekChar();
                if ((c < 0x30) || (0x39 < c))
                {
                    break;
                }
                val = (val * 10M) + (c - 0x30);
                this.ReadChar();
                if ((zeroStart && (x == 1)) && (c == 0x30))
                {
                    throw this.JsonError("leading multiple zeros are not allowed");
                }
                x++;
            }
            bool hasFrac = false;
            decimal frac = 0M;
            int fdigits = 0;
            if (this.PeekChar() == 0x2e)
            {
                hasFrac = true;
                this.ReadChar();
                if (this.PeekChar() < 0)
                {
                    throw this.JsonError("Invalid JSON numeric literal; extra dot");
                }
                decimal d = 10M;
                while (true)
                {
                    c = this.PeekChar();
                    if ((c < 0x30) || (0x39 < c))
                    {
                        break;
                    }
                    this.ReadChar();
                    frac += (c - 0x30) / d;
                    d *= 10M;
                    fdigits++;
                }
                if (fdigits == 0)
                {
                    throw this.JsonError("Invalid JSON numeric literal; extra dot");
                }
            }
            frac = decimal.Round(frac, fdigits);
            switch (this.PeekChar())
            {
                case 0x65:
                case 0x45:
                    this.ReadChar();
                    exp = 0;
                    if (this.PeekChar() < 0)
                    {
                        throw new ArgumentException("Invalid JSON numeric literal; incomplete exponent");
                    }
                    switch (this.PeekChar())
                    {
                        case 0x2d:
                            this.ReadChar();
                            break;

                        case 0x2b:
                            this.ReadChar();
                            break;
                    }
                    break;

                default:
                {
                    if (!hasFrac)
                    {
                        if ((negative && (-2147483648M <= -val)) || (!negative && (val <= 2147483647M)))
                        {
                            return (negative ? ((int) -val) : ((int) val));
                        }
                        if ((negative && (-9223372036854775808M <= -val)) || (!negative && (val <= 9223372036854775807M)))
                        {
                            return (negative ? ((long) -val) : ((long) val));
                        }
                    }
                    decimal v = val + frac;
                    return (negative ? -v : v);
                }
            }
            if (this.PeekChar() < 0)
            {
                throw this.JsonError("Invalid JSON numeric literal; incomplete exponent");
            }
        Label_025D:
            c = this.PeekChar();
            if ((c >= 0x30) && (0x39 >= c))
            {
                exp = (exp * 10) + (c - 0x30);
                this.ReadChar();
                goto Label_025D;
            }
            int[] bits = decimal.GetBits(val + frac);
            return new decimal(bits[0], bits[1], bits[2], negative, (byte) exp);
        }

        private string ReadStringLiteral()
        {
            int c;
            if (this.PeekChar() != 0x22)
            {
                throw this.JsonError("Invalid JSON string literal format");
            }
            this.ReadChar();
            this.vb.Length = 0;
        Label_0029:
            c = this.ReadChar();
            if (c < 0)
            {
                throw this.JsonError("JSON string is not closed");
            }
            if (c == 0x22)
            {
                return this.vb.ToString();
            }
            if (c == 0x5c)
            {
                c = this.ReadChar();
                if (c < 0)
                {
                    throw this.JsonError("Invalid JSON string literal; incomplete escape sequence");
                }
                switch (c)
                {
                    case 0x22:
                    case 0x2f:
                    case 0x5c:
                        this.vb.Append((char) c);
                        goto Label_0029;

                    case 0x72:
                        this.vb.Append('\r');
                        goto Label_0029;

                    case 0x74:
                        this.vb.Append('\t');
                        goto Label_0029;

                    case 0x75:
                    {
                        ushort cp = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            cp = (ushort) (cp << 4);
                            c = this.ReadChar();
                            if (c < 0)
                            {
                                throw this.JsonError("Incomplete unicode character escape literal");
                            }
                            if ((0x30 <= c) && (c <= 0x39))
                            {
                                cp = (ushort) (cp + ((ushort) (c - 0x30)));
                            }
                            if ((0x41 <= c) && (c <= 70))
                            {
                                cp = (ushort) (cp + ((ushort) ((c - 0x41) + 10)));
                            }
                            if ((0x61 <= c) && (c <= 0x66))
                            {
                                cp = (ushort) (cp + ((ushort) ((c - 0x61) + 10)));
                            }
                        }
                        this.vb.Append((char) cp);
                        goto Label_0029;
                    }
                    case 110:
                        this.vb.Append('\n');
                        goto Label_0029;

                    case 0x62:
                        this.vb.Append('\b');
                        goto Label_0029;

                    case 0x66:
                        this.vb.Append('\f');
                        goto Label_0029;
                }
                throw this.JsonError("Invalid JSON string literal; unexpected escape character");
            }
            this.vb.Append((char) c);
            goto Label_0029;
        }

        private void SkipSpaces()
        {
        Label_0000:
            switch (this.PeekChar())
            {
                case 9:
                case 10:
                case 13:
                case 0x20:
                    this.ReadChar();
                    goto Label_0000;

                case 11:
                case 12:
                    return;
            }
        }
    }
}

