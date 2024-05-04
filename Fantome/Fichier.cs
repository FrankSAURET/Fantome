using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fantome
{
    internal static class Fichier
    {
        /// <summary>
        /// Conversion d'un icône en image
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        /// <summary>
        /// Import dll
        /// </summary>
        /// <param name="lptstrFilename"></param>
        /// <param name="lpdwHandle"></param>
        /// <returns></returns>

        [DllImport("version.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetFileVersionInfoSize(string lptstrFilename, out int lpdwHandle);

        [DllImport("version.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetFileVersionInfo(string lptstrFilename, int dwHandle, int dwLen, byte[] lpData);

        [DllImport("version.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen);

        /// <summary>
        /// Récupération des info. d'un fichier
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        /// Usage :
        /// var infos = GetVersionInfo(name, "LastModified", "Comments", "CompanyName", "FileVersion", "LegalCopyright", "LegalTrademarks", "ProductVersion", "InternalName", "OriginalFilename", "FileDescription", "ProductName", "BuildDate");
        /// ou
        /// var buildDate = infos.Single(x => x.Item1 == "BuildDate").Item2;
        public static Tuple<string, string>[] GetVersionInfo(string fileName, params string[] keys)
        {
            int num;
            int size = GetFileVersionInfoSize(fileName, out num);

            if (size == 0)
            {
                throw new Win32Exception();
            }

            var bytes = new byte[size];
            bool success = GetFileVersionInfo(fileName, 0, size, bytes);

            if (!success)
            {
                throw new Win32Exception();
            }

            int size2;
            IntPtr ptr;

            success = VerQueryValue(bytes, @"\VarFileInfo\Translation", out ptr, out size2);

            uint[] langs;

            if (success)
            {
                langs = new uint[size2 / 4];

                for (int i = 0, j = 0; j < size2; i++, j += 4)
                {
                    langs[i] = unchecked((uint)(((ushort)Marshal.ReadInt16(ptr, j) << 16) | (ushort)Marshal.ReadInt16(ptr, j + 2)));
                }
            }
            else
            {
                langs = new uint[] { 0x040904B0, 0x040904E4, 0x04090000 };
            }

            string[] langs2 = Array.ConvertAll(langs, x => @"\StringFileInfo\" + x.ToString("X8") + @"\");

            var kv = new Tuple<string, string>[keys.Length];

            for (int i = 0; i < kv.Length; i++)
            {
                string key = keys[i];
                string value = null;

                foreach (var lang in langs2)
                {
                    success = VerQueryValue(bytes, lang + key, out ptr, out size2);

                    if (success)
                    {
                        value = Marshal.PtrToStringUni(ptr);
                        break;
                    }
                }

                kv[i] = Tuple.Create(key, value);
            }

            return kv;
        }
    }
}