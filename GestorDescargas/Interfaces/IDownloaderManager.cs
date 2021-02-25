using GestorDescargas.Models;
using System.Collections.Generic;
using System.IO;

namespace GestorDescargas.Interfaces
{
    public interface IDownloaderManager
    {

        string GetJsonFromURL(string url);
        StructureFolderFile GetStructureFromJson(string json);
        bool CopyFileToFolder(Archive archive, Configuration config);
        bool DownloadFileWithRange(string url, string filePath);
        bool IsNewFile(Archive file, string savePath);
        string GetMD5HashFromFile(FileStream fileStream);
        bool IsFile(dynamic node);

        int GetTotalFilesDownloaded();

    }
}