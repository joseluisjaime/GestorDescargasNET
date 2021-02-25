using GestorDescargas.Interfaces;
using GestorDescargas.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GestorDescargas.Services
{
    public class DownloaderManager : IDownloaderManager
    {
        public ILogger _logger;
        public IDownloaderManagerRepository _downloadManagerRepository;
        public int TotalFilesDownloaded;

        public DownloaderManager(ILogger logger, IDownloaderManagerRepository downloadManagerRepository)
        {
            _logger = logger;
            _downloadManagerRepository = downloadManagerRepository;
        }

        public string GetJsonFromURL(string url)
        {

            string jsonDownloaded;
            try
            {
                jsonDownloaded = _downloadManagerRepository.GetJsonFromURL(url);
            }
            catch (Exception e)
            {
                _logger.WriteMessage($"-----Error reading file from url: {url}-----\nErrors detail: {e.Message}");
                throw e;
            }

            return jsonDownloaded;
        }

        public StructureFolderFile GetStructureFromJson(string json)
        {
            List<Folder> folderList = new List<Folder>();
            List<Archive> archiveList = new List<Archive>();

            try
            {
                List<JToken> jsonList = JObject.Parse(json).Descendants()
                .Where(x => x is JProperty)
                .ToList();


                foreach (dynamic node in jsonList)
                {
                    //check if the node is a file
                    if (IsFile(node))
                    {
                        if (node.Name == "md5")
                        {
                            archiveList.Add(new Archive
                            {
                                Md5 = node.Value.Value,
                                Name = node.Next.Value.Value,
                                Required = node.Next.Next.Value.Value,
                                Path = Utils.FormatFilePath(node.Path)
                            });
                        }
                    }
                    else
                    {
                        folderList.Add(new Folder
                        {
                            Name = node.Name,
                            Path = Utils.FormatFolderPath(node.Path)
                        });

                    }
                }
            } catch (Exception e)
            {
                _logger.WriteMessage($"------Invalid json format-------\nErrors detail: {e.Message}");
                throw e;
            }
            

            return new StructureFolderFile
            {
                ListArchives = archiveList,
                ListFolders = folderList
            };

        }

        public bool CopyFileToFolder(Archive file, Configuration config)
        {
            bool copied = false;
            string urlWithoutFile = Utils.UrlWithoutFile(config.Url);
            try
            {
                if (file.Required)
                {
                    if (IsNewFile(file, config.SavePath))
                    {
                        copied = DownloadFileWithRange(urlWithoutFile + file.Path + file.Name, config.SavePath + file.Path + file.Name);
                        this.TotalFilesDownloaded += 1;
                    }
                }
            } catch (Exception e)
            {
                throw e;
            }

            return copied;
        }

        public bool DownloadFileWithRange(string url, string filePath)
        {
            bool downloaded;
            try
            {
                downloaded = _downloadManagerRepository.DownloadFileWithRange(url, filePath);
            }
            catch (WebException e)
            {
                _logger.WriteMessage($"Cant get file from: {url}---\nError Detail: - {e.Message}");
                downloaded = false;
            }
            catch (Exception e)
            {
                _logger.WriteMessage($"Cant create or read file: {filePath}");

                throw e;
            }

            return downloaded;
        }

        public bool IsNewFile(Archive file, string savePath)
        {
            bool isNewFile;
            string filePath = savePath + file.Path + file.Name;
            try
            {
                isNewFile = _downloadManagerRepository.IsNewFile(file, savePath);
            }
            catch (Exception e)
            {
                _logger.WriteMessage($"Problem accesing file: {filePath}");
                throw e;
            }

            return isNewFile;
        }

        public string GetMD5HashFromFile(FileStream fileStream)
        {
            return _downloadManagerRepository.GetMD5HashFromFile(fileStream);
        }

        public bool IsFile(dynamic node)
        {
            string name = node.Name;
            bool isFile = false;
            //with this if i am cheking if are some folder called "md5", "name" or "required", a file always have .Next except the las node, required, that must have a previous called name
            if (node.Next != null || (node.Previous != null && node.Previous.Name == "name"))
            {
                switch (name)
                {
                    case "md5":
                    case "name":
                    case "required":
                        isFile = true;
                        break;
                }
            }

            return isFile;
        }

        public int GetTotalFilesDownloaded()
        {
            return this.TotalFilesDownloaded;
        }
    }
}

