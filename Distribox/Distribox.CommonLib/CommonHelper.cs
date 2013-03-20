using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Threading;

namespace Distribox.CommonLib
{
    public class CommonHelper
    {
        private static Random rd = new Random();

        public static String GetSHA1Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            SHA1CryptoServiceProvider oSHA1Hasher = new SHA1CryptoServiceProvider();

            while (true)
            {
                try
                {
                    oFileStream = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    arrbytHashValue = oSHA1Hasher.ComputeHash(oFileStream);
                    oFileStream.Close();
                    break;
                }
                catch
                {
                    Thread.Sleep(1);
                }
            }

            strHashData = BitConverter.ToString(arrbytHashValue);
            strHashData = strHashData.Replace("-", "");
            strResult = strHashData;

            return (strResult);
        }

        public static String GetRandomHash()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyyMMddHHmmss") + now.Millisecond.ToString() + rd.Next(10000).ToString();
        }

        public static T Read<T>(String input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static T Read<T>(byte[] input)
        {
            return Read<T>(ByteToString(input));
        }

        public static String Show(Object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

        public static byte[] ShowAsBytes(Object input)
        {
            return StringToByte(Show(input));
        }

        public static String ReadFile(String filename)
        {
            StreamReader fin = new StreamReader(filename);
            String ret = fin.ReadToEnd();
            fin.Close();
            return ret;
        }

        public static void WriteFile(String filename, String text)
        {
            StreamWriter fout = new StreamWriter(filename);
            fout.Write(text);
            fout.Flush();
            fout.Close();
        }

        public static T ReadObject<T>(String filename)
        {
            String str = ReadFile(filename);
            return Read<T>(str);
        }

        public static void WriteObject(String filename, Object input)
        {
            String str = Show(input);
            WriteFile(filename, str);
        }

        public static Byte[] StringToByte(String str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static String ByteToString(Byte[] bytes)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(bytes);
        }
    }
}
