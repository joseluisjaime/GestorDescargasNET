using GestorDescargas.Models;
using System.Collections.Generic;
using System.IO;


namespace GestorDescargas.Interfaces
{
    public interface IDownloaderManagerRepository
    {

        string GetJsonFromURL(string url);
        bool DownloadFileWithRange(string url, string filePath);
        bool IsNewFile(Archive file, string savePath);
        string GetMD5HashFromFile(FileStream fileStream);

    }
}