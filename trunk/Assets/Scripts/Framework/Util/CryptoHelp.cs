using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Framework.Util {

    public static class CryptoHelp {

        public static string MD5(string ts, string key = "") {
            string cs = ts + key;
            byte[] input = Encoding.UTF8.GetBytes(cs);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(input);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in output) {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static string MD5(Stream input) {
            MD5 md5 = new MD5CryptoServiceProvider();
            input.Position = 0;
            byte[] output = md5.ComputeHash(input);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in output) {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static string MD5(byte[] input, int startIndex, int length) {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(input, startIndex, length);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in output) {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static string Base64Encode(string ss, int index) {
            byte[] bytes = Encoding.Default.GetBytes(ss);
            return Convert.ToBase64String(bytes);
        }
    }
}
