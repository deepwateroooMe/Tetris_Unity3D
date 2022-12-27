using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Core;

namespace Framework.Util {

    // 进行必要要的压缩与解压缩
    public static class CompressExtension {

        public static void Compress(FileInfo fileToCompress) {
            using (FileStream fs = File.OpenRead(fileToCompress.FullName)) {
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz") {
                    using (GZipOutputStream s = new GZipOutputStream(File.Create(fileToCompress.FullName + ".gz"))) {
                        byte[] writeData = new byte[4096];
                        StreamUtils.Copy(fs, s, writeData);
                    }
                }
            }
        }

        public static byte[] Compress(byte[] datas, int startIndex, int length) {
            using (MemoryStream ms = new MemoryStream()) {
                using (GZipOutputStream gzip = new GZipOutputStream(ms)) {
                    gzip.Write(datas, 0, datas.Length);
                }
                datas = ms.ToArray();
            }
            return datas;
        }

        public static byte[] DeCompress(byte[] datas, int startIndex, int length) {
            using (MemoryStream ms = new MemoryStream(datas, startIndex, length)) {
                using (GZipInputStream gzis = new GZipInputStream(ms)) {
                    using (MemoryStream re = new MemoryStream()) {
                        int count = 0;
                        byte[] data = new byte[4096];
                        while ((count = gzis.Read(data, 0, data.Length)) != 0) {
                            re.Write(data, 0, count);
                        }
                        return re.ToArray();
                    }
                }
            }
        }
    }
}
