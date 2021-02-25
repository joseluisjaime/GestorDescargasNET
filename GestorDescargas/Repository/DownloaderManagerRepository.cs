using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using GestorDescargas.Interfaces;
using GestorDescargas.Models;

namespace GestorDescargas.Repository
{
    class DownloaderManagerRepository : IDownloaderManagerRepository
    {

        public bool DownloadFileWithRange(string url, string filePath)
        {
            long totalBytesRead = 0;
            long MaxContentLength = 0;
            long RequestContentLength = 0;
            try
            {
                using (FileStream localFileStream = new FileStream(filePath, FileMode.Append))
                {
                    totalBytesRead = localFileStream.Length;

                    while (MaxContentLength == 0 || totalBytesRead < MaxContentLength)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                        if (totalBytesRead > 0)
                        {
                            request.AddRange(totalBytesRead);
                        }
                        try
                        {
                            WebResponse response = request.GetResponse();

                            if (response.ContentLength > MaxContentLength)
                            {
                                MaxContentLength = response.ContentLength;
                            }

                            using (var responseStream = response.GetResponseStream())
                            {
                                var buffer = new byte[4096];
                                int bytesRead;

                                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    totalBytesRead += bytesRead;
                                    RequestContentLength += bytesRead;
                                    localFileStream.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                        catch (WebException)
                        {
                            return false;
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetJsonFromURL(string url)
        {
            string jsonDownloaded;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonDownloaded = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return jsonDownloaded;
        }

        public string GetMD5HashFromFile(FileStream fileStream)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", string.Empty).ToLower();
            }
        }

        public bool IsNewFile(Archive file, string savePath)
        {
            bool isNewFile = true;
            string filePath = savePath + file.Path + file.Name;
            try
            {
                if (File.Exists(filePath))
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        string localFileMD5 = GetMD5HashFromFile(fileStream);
                        if (localFileMD5.Equals(file.Md5))
                        {
                            isNewFile = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return isNewFile;
        }
    }
}
