using GestorDescargas.Interfaces;
using GestorDescargas.Models;
using GestorDescargas.Repository;
using GestorDescargas.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GestorDescargas
{
    class Program
    {

        private static IDownloaderManager _downloaderManager;
        private static ILogger _logger;
        private static IFolderManager _folderManager;
        static void Main(string[] args)
        {
            InitConfiguration();


            Configuration configuration = ReadingParameters(args);


            _logger.WriteMessage("Starting program......");



            if (Utils.IsValidURL(configuration.Url))
            {

                try
                {
                    _logger.WriteMessage($"downloading Json from: {configuration.Url}");
                    string json = _downloaderManager.GetJsonFromURL(configuration.Url);

                    //Get structure of folders and files
                    StructureFolderFile structure = _downloaderManager.GetStructureFromJson(json);
                    _logger.WriteMessage($"Starting to download files....");



                    CreateFolders(structure, configuration);

                    CopyFiles(structure, configuration);
                }
                catch (Exception) {
                    _logger.WriteMessage("Closing program....");
                }
            }
            else
            {
                _logger.WriteMessage("Invalid URL");
            }
        }



        public static void InitConfiguration()
        {

            var serviceProvider = new ServiceCollection()
           .AddSingleton<IDownloaderManager, DownloaderManager>()
           .AddSingleton<ILogger, Logger>()
           .AddSingleton<IFolderManager, FolderManager>()
           .AddSingleton<IDownloaderManagerRepository, DownloaderManagerRepository>()
           .BuildServiceProvider();

            _downloaderManager = serviceProvider.GetService<IDownloaderManager>();
            _logger = serviceProvider.GetService<ILogger>();
            _folderManager = serviceProvider.GetService<IFolderManager>();
        }

        private static Configuration ReadingParameters(string[] args)
        {
            string url;
            string savePath;

            if (args.Count() == 2)
            {
                url = args[0];
                savePath = args[1];

            }
            else
            {
                Console.Write("Please enter the URL: ");
                url = Console.ReadLine();
                Console.Write("Enter the directory where you want to save the files: ");
                savePath = Console.ReadLine();

            }

            return new Configuration
            {
                Url = url,
                SavePath = savePath
            };
        }

        public static void CreateFolders(StructureFolderFile structure, Configuration configuration)
        {
            //Create Folders if not exist
            try
            {
                foreach (Folder folder in structure.ListFolders)
                {
                    _folderManager.CreateFolders(folder, configuration);
                }
            }
            catch (Exception e)
            {
                _logger.WriteMessage("Cant create Folders, check permissions");
                throw e;
            }
        }

        public static void CopyFiles(StructureFolderFile structure, Configuration configuration)
        {
            try
            {
                double totalArchives = structure.ListArchives.Count();
                double counterIncrement = 1 / totalArchives;
                double counterProgress = 0;

                using (var progress = new ProgressBar())
                {
                    foreach (Archive archive in structure.ListArchives)
                    {

                        progress.Report(counterProgress);
                        _downloaderManager.CopyFileToFolder(archive, configuration);
                        counterProgress += counterIncrement;
                    }
                }

                //_progressBar.Report(1);
                _logger.WriteMessage("\nDownloads complete........");
                _logger.WriteMessage($"Total files downloaded: {_downloaderManager.GetTotalFilesDownloaded()}........");

            }
            catch (Exception e)
            {
                _logger.WriteMessage($"Problem copiying the files, error: \n{e.Message}");
            }
        }
    }
}
