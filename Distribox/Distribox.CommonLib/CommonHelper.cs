using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

namespace Distribox.CommonLib
{
	/// <summary>
	/// Common helpers.
	/// </summary>
    public static class CommonHelper
    {
		/// <summary>
		/// Ramdom Generator
		/// </summary>
        private static Random _rd = new Random();

		/// <summary>
		/// Gets the SHA1 hash of a file.
		/// </summary>
		/// <returns>The SHA1 hash.</returns>
		/// <param name="pathName">Path name.</param>
        public static string GetSHA1Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            SHA1CryptoServiceProvider oSHA1Hasher = new SHA1CryptoServiceProvider();

			// TODO comment for find another solution
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

		/// <summary>
		/// Gets a random hash.
		/// </summary>
		/// <returns>The random hash.</returns>
        public static string GetRandomHash()
        {
            byte[] ticks = BitConverter.GetBytes(DateTime.Now.Ticks);
            byte[] random = BitConverter.GetBytes(_rd.Next());

            byte[] bytes = new byte[12];
            ticks.CopyTo(bytes, 0);
            random.CopyTo(bytes, ticks.Length);

            return Convert.ToBase64String(bytes).Replace("/", "@");
        }

		/// <summary>
		/// Deserialize the specified input.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Deserialize<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

		/// <summary>
		/// Deserialize the specified input.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Deserialize<T>(this byte[] input)
        {
            return Deserialize<T>(ByteToString(input));
        }

		/// <summary>
		/// Serialize the specified input.
		/// </summary>
		/// <param name="input">Input.</param>
        public static string Serialize(this Object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

		/// <summary>
		/// Serializes as bytes.
		/// </summary>
		/// <returns>The as bytes.</returns>
		/// <param name="input">Input.</param>
        public static byte[] SerializeAsBytes(this Object input)
        {
            return StringToByte(Serialize(input));
        }

		/// <summary>
		/// Reads the object from file.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="filename">Filename.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T ReadObject<T>(string filename)
        {
            string str = File.ReadAllText(filename);
            return Deserialize<T>(str);
        }

		/// <summary>
		/// Writes the object to file.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <param name="filename">Filename.</param>
		public static void WriteObject(this Object input, string filename)
        {
			string str = input.Serialize();
			File.WriteAllText(filename, str);
        }

		/// <summary>
		/// Converts string to bytes.
		/// </summary>
		/// <returns>The to byte.</returns>
		/// <param name="str">String.</param>
        public static byte[] StringToByte(string str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

		/// <summary>
		/// Converts bytes to string.
		/// </summary>
		/// <returns>The to string.</returns>
		/// <param name="bytes">Bytes.</param>
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
		/// Initialize all the folders and files in monitered file.
		/// </summary>
		/// <param name="root">Path of root folder.</param>
		public static void InitializeFolder(string root)
		{
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}
			if (!Directory.Exists(root + Properties.MetaFolder))
			{
				Directory.CreateDirectory(root + Properties.MetaFolder);
			}
			if (!Directory.Exists(root + Properties.MetaFolderTmp))
			{
				Directory.CreateDirectory(root + Properties.MetaFolderTmp);
			}
			if (!Directory.Exists(root + Properties.MetaFolderData))
			{
				Directory.CreateDirectory(root + Properties.MetaFolderData);
			}
			if (!File.Exists(root + Properties.VersionListFilePath))
			{
				File.WriteAllText(root + Properties.VersionListFilePath, "[]");
			}
		}
    }
}
