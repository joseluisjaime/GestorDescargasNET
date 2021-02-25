using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GestorDescargas.Services
{
    public class Utils
    {

        public static bool IsValidURL(string url)
        {
            Uri uriResult;
            return  Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        //Replace characters that do not interest us to form the path ej: common[0].ptf_Terminal[1] -> common/ptf_Terminal
        public static string FormatFolderPath(string path)
        {
            return Regex.Replace(path, @"\[\d+\].", "/");
        }

        //format the file part ex: common/ptf_Terminal/help/en_GB/md5
        public static string FormatFilePath(string path)
        {
            //format the folder part
            string pathFormatted = FormatFolderPath(path);
            return Regex.Replace(pathFormatted, @"md5$", "");
        }

        public static string UrlWithoutFile(string url)
        {
            return Regex.Replace(url, @"((?=\w+\.\w{3,4}$).+)", "");
        }
    }
}
