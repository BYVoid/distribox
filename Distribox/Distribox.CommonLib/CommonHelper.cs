//-----------------------------------------------------------------------
// <copyright file="CommonHelper.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CommonLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using ICSharpCode.SharpZipLib.Zip;
    using Newtonsoft.Json;

    /// <summary>
    /// Common helpers.
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        /// Random Generator
        /// </summary>
        private static Random rd = new Random();

        /// <summary>
        /// Gets the SHA1 hash of a file.
        /// </summary>
        /// <returns>The SHA1 hash.</returns>
        /// <param name="pathName">Path name.</param>
        public static string GetSHA1Hash(string pathName)
        {
            string strResult = string.Empty;
            string strHashData = string.Empty;

            byte[] arrbytHashValue;
            System.IO.FileStream fileStream = null;

            SHA1CryptoServiceProvider sha1Hasher = new SHA1CryptoServiceProvider();

            // TODO comment for find another solution
            while (true)
            {
                try
                {
                    fileStream = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    arrbytHashValue = sha1Hasher.ComputeHash(fileStream);
                    fileStream.Close();
                    break;
                }
                catch
                {
                    Thread.Sleep(1);
                }
            }

            strHashData = BitConverter.ToString(arrbytHashValue);
            strHashData = strHashData.Replace("-", string.Empty);
            strResult = strHashData;

            return strResult;
        }

        /// <summary>
        /// Gets a random hash.
        /// </summary>
        /// <returns>The random hash.</returns>
        public static string GetRandomHash()
        {
            return rd.Next().ToString();

            byte[] ticks = BitConverter.GetBytes(DateTime.Now.Ticks);
            byte[] random = BitConverter.GetBytes(rd.Next());

            byte[] bytes = new byte[12];
            ticks.CopyTo(bytes, 0);
            random.CopyTo(bytes, ticks.Length);

            return Convert.ToBase64String(bytes).Replace("/", "@");
        }

        /// <summary>
        /// Deserialize the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <returns>Deserialize result.</returns>
        public static T Deserialize<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        /// <summary>
        /// Deserialize the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <returns>Deserialize result.</returns>
        public static T Deserialize<T>(this byte[] input)
        {
            return Deserialize<T>(ByteToString(input));
        }

        /// <summary>
        /// Serialize the specified input inline.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Serialize result.</returns>
        public static string SerializeInline(this object input)
        {
            return JsonConvert.SerializeObject(input);
        }

        /// <summary>
        /// Serialize the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Serialize result.</returns>
        public static string Serialize(this object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

        /// <summary>
        /// Serializes as bytes.
        /// </summary>
        /// <returns>The as bytes.</returns>
        /// <param name="input">The input.</param>
        /// <returns>Serialize result.</returns>
        public static byte[] SerializeAsBytes(this object input)
        {
            return StringToByte(Serialize(input));
        }

        /// <summary>
        /// Reads the object from file.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="filename">The filename.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T ReadObject<T>(string filename)
        {
            string str = File.ReadAllText(filename);
            return Deserialize<T>(str);
        }

        /// <summary>
        /// Writes the object to file.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="filename">The filename.</param>
        public static void WriteObject(this object input, string filename)
        {
            string str = input.Serialize();
            File.WriteAllText(filename, str);
        }

        /// <summary>
        /// Converts string to bytes.
        /// </summary>
        /// <returns>The to byte.</returns>
        /// <param name="str">The string.</param>
        public static byte[] StringToByte(string str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Converts bytes to string.
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The string.</returns>
        public static string ByteToString(byte[] bytes)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Compress the specified <paramref name="sourceDirectory"/> into <paramref name="zipFileName"/>.
        /// </summary>
        /// <param name="zipFileName">Zip file name.</param>
        /// <param name="sourceDirectory">Source directory.</param>
        public static void Zip(string zipFileName, string sourceDirectory)
        {
            FastZip zip = new FastZip();
            zip.CreateZip(zipFileName, sourceDirectory, true, null);
        }

        /// <summary>
        /// Decompress the <paramref name="zipFileName"/> into <paramref name="targetDirectory"/>.
        /// </summary>
        /// <param name="zipFileName">Zip file name.</param>
        /// <param name="targetDirectory">Target directory.</param>
        public static void UnZip(string zipFileName, string targetDirectory)
        {
            FastZip zip = new FastZip();
            zip.ExtractZip(zipFileName, targetDirectory, null);
        }

        /// <summary>
        /// Initialize all the folders and files in monitored file.
        /// </summary>
        public static void InitializeFolder()
        {
            if (!Directory.Exists(Config.RootFolder))
            {
                Directory.CreateDirectory(Config.RootFolder);
            }

            if (!Directory.Exists(Config.MetaFolder))
            {
                Directory.CreateDirectory(Config.MetaFolder);
            }

            if (!Directory.Exists(Config.MetaFolderTmp))
            {
                Directory.CreateDirectory(Config.MetaFolderTmp);
            }

            if (!Directory.Exists(Config.MetaFolderData))
            {
                Directory.CreateDirectory(Config.MetaFolderData);
            }

            if (!File.Exists(Config.VersionListFilePath))
            {
                File.WriteAllText(Config.VersionListFilePath, "[]");
            }
        }
    }
}
