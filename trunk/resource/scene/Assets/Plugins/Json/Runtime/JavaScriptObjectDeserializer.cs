namespace System.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using System.Text;
 
    internal class JavaScriptObjectDeserializer
    {
        private JavaScriptReader reader;

        public JavaScriptObjectDeserializer(string json, bool raiseNumberParseError)
        {
            this.reader = new JavaScriptReader(new StringReader(json), raiseNumberParseError);
        }

        public object BasicDeserialize()
        {
            return this.reader.Read();
        }

        public static Encoding DetectEncoding(int byte1, int byte2)
        {
            if (byte1 == 0)
            {
                if (byte2 == 0)
                {
                    throw new Exception("UTF-32BE is detected, which is not supported");
                }
                return Encoding.BigEndianUnicode;
            }
            if (byte2 == 0)
            {
                return Encoding.Unicode;
            }
            return Encoding.UTF8;
        }

        private class BufferedStream : Stream
        {
            private int first;
            private long pos;
            private int second;
            private Stream source;

            public BufferedStream(Stream source)
            {
                this.source = source;
                this.first = source.ReadByte();
                this.second = source.ReadByte();
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int index, int count)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if ((index < 0) || (index >= buffer.Length))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if ((count < 0) || ((index + count) >= buffer.Length))
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if (count == 0)
                {
                    return 0;
                }
                if (this.pos < 2)
                {
                    buffer[(int) ((IntPtr) this.pos)] = (this.pos == 0) ? ((byte) this.first) : ((byte) this.second);
                    this.pos++;
                    return (this.Read(buffer, index + 1, count - 1) + 1);
                }
                return this.source.Read(buffer, index, count);
            }

            public override int ReadByte()
            {
                long num = this.pos;
                if ((num  <= 1) && (num  >= 0))
                {
                    switch (((int)num))
                    {
                        case 0:
                            this.pos++;
                            return this.first;

                        case 1:
                            this.pos++;
                            return this.second;
                    }
                }
                return this.source.ReadByte();
            }

            public override long Seek(long pos, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long pos)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buf, int index, int count)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead
            {
                get
                {
                    return this.source.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            public override long Length
            {
                get
                {
                    return this.source.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return this.source.Position;
                }
                set
                {
                    if (value < 2)
                    {
                        this.pos = value;
                    }
                    this.source.Position = value;
                }
            }
        }

        //public class BufferedStreamReader : StreamReader
        //{
        //    public BufferedStreamReader(Stream stream) : base(stream, JavaScriptObjectDeserializer.DetectEncoding(stream.ReadByte(), stream.ReadByte()))
        //    {
        //    }
        //}
    }
}

