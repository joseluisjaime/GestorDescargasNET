using NUnit.Framework;
using GestorDescargas.Services;
using Moq;
using GestorDescargas.Interfaces;
using System.Net;
using GestorDescargas.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace GestorDescargasTests
{
    public class DownloaderManagerTests
    {

        [Test]
        public void DownloadFileWithRange_ValidFile_ShouldReturnTrue()
        {
            // Arrange
            string url = "http://172.0.0.1/assetsTree.json";
            string path = "Descargas/";
            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.DownloadFileWithRange(url, path)).Returns(true);

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.DownloadFileWithRange(url, path);
            // Assert
            Assert.IsTrue(response);
        }

        [Test]
        public void DownloadFileWithRange_CantAccessServerFile_ShouldReturnFalse()
        {
            // Arrange
            string url = "http://172.1.1.1/assetsTree.json";
            string path = "Descargas/";
            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.DownloadFileWithRange(url, path)).Throws(new WebException());

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.DownloadFileWithRange(url, path);
            // Assert
            Assert.IsFalse(response);
        }

        [Test]
        public void GetStructureFromJson_ValidJson_ShouldReturnValidObject()
        {
            // Arrange
            string validJson = "{ \"common\": [ { \"ptf_Terminal\": [ { \"help\": [ { \"en_GB\": [ { \"md5\": \"fd632797a9adb10d068868f86a2b6951\", \"name\": \"keno.html\", \"required\": true }, { \"md5\": \"4262186789c88657ddeaef6acbaaa45f\", \"name\": \"roulette.html\", \"required\": true } ] } ] } ] } ] }";

            List<Folder> folders = new List<Folder>();
            List<Archive> files = new List<Archive>();

            folders.Add(new Folder { Name = "common", Path = "common" });
            folders.Add(new Folder { Name = "ptf_Terminal", Path = "common/ptf_Terminal" });
            folders.Add(new Folder { Name = "help", Path = "common/ptf_Terminal/help" });
            folders.Add(new Folder { Name = "en_GB", Path = "common/ptf_Terminal/help/en_GB" });

            files.Add(new Archive { Md5 = "fd632797a9adb10d068868f86a2b6951", Name = "keno.html", Required = true, Path = "common/ptf_Terminal/help/en_GB/" });
            files.Add(new Archive { Md5 = "4262186789c88657ddeaef6acbaaa45f", Name = "roulette.html", Required = true, Path = "common/ptf_Terminal/help/en_GB/" });

            StructureFolderFile structureExpected = new StructureFolderFile { ListArchives = files, ListFolders = folders };

            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            StructureFolderFile response = downloaderManager.GetStructureFromJson(validJson);

            // Assert
            Assert.AreEqual(response.ListArchives.Count(), structureExpected.ListArchives.Count());
            Assert.AreEqual(response.ListFolders.Count(), structureExpected.ListFolders.Count());
        }
        [Test]
        public void GetJsonFromURL_ValidStringJson_ShouldReturnValidStringJson()
        {
            // Arrange
            string url = "http://172.0.0.1/assetsTree.json";
            string validJson = "{ \"common\": [ { \"ptf_Terminal\": [ { \"help\": [ { \"en_GB\": [ { \"md5\": \"fd632797a9adb10d068868f86a2b6951\", \"name\": \"keno.html\", \"required\": true }, { \"md5\": \"4262186789c88657ddeaef6acbaaa45f\", \"name\": \"roulette.html\", \"required\": true } ] } ] } ] } ] }";


            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();

            downloaderManagerRepo.Setup(dm => dm.GetJsonFromURL(url)).Returns(validJson);

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);
            // Act
            string response = downloaderManager.GetJsonFromURL(url);
            // Assert
            Assert.AreEqual(response, validJson);
        }
        [Test]
        public void GetJsonFromURL_ConnectionFail_ShouldReturnException()
        {
            // Arrange
            string url = "http://172.0.0.1/assetsTree.json";
            string validJson = "{ \"common\": [ { \"ptf_Terminal\": [ { \"help\": [ { \"en_GB\": [ { \"md5\": \"fd632797a9adb10d068868f86a2b6951\", \"name\": \"keno.html\", \"required\": true }, { \"md5\": \"4262186789c88657ddeaef6acbaaa45f\", \"name\": \"roulette.html\", \"required\": true } ] } ] } ] } ] }";


            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();

            downloaderManagerRepo.Setup(dm => dm.GetJsonFromURL(url)).Throws(new Exception());

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);
            // Act
            // Assert
            Assert.Throws<Exception>(
                () => { downloaderManager.GetJsonFromURL(url); });
        }
        [Test]
        public void IsNewFile_FileIsNew_ShouldReturnTrue()
        {
            // Arrange
            string path = "Descargas/";

            Archive file = new Archive { Md5 = "fd632797a9adb10d068868f86a2b6951", Name = "keno.html", Required = true, Path = "common/ptf_Terminal/help/en_GB/" };
            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.IsNewFile(file, path)).Returns(true);

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.IsNewFile(file, path);
            // Assert
            Assert.IsTrue(response);
        }
        [Test]
        public void IsNewFile_FileNotNew_ShouldReturnFalse()
        {
            // Arrange
            string path = "Descargas/";

            Archive file = new Archive { Md5 = "fd632797a9adb10d068868f86a2b6951", Name = "keno.html", Required = true, Path = "common/ptf_Terminal/help/en_GB/" };
            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.IsNewFile(file, path)).Returns(false);

            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.IsNewFile(file, path);
            // Assert
            Assert.IsFalse(response);
        }
        [Test]
        public void CopyFileToFolder_FileRequired_ShouldReturnTrue()
        {
            // Arrange
            string path = "Descargas/";
            string url = "http://172.0.0.1/assetsTree.json";
            string urlWithoutFile = "http://172.0.0.1/";
            Configuration config = new Configuration { Url = url, SavePath = path};

            Archive file = new Archive { Md5 = "fd632797a9adb10d068868f86a2b6951", Name = "keno.html", Required = true, Path = "common/ptf_Terminal/help/en_GB/" };

            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.IsNewFile(file, config.SavePath)).Returns(true);
            downloaderManagerRepo.Setup(a => a.DownloadFileWithRange(urlWithoutFile + file.Path + file.Name, config.SavePath + file.Path + file.Name)).Returns(true);


            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.CopyFileToFolder(file, config);
            // Assert
            Assert.IsTrue(response);
        }
        [Test]
        public void CopyFileToFolder_FileNotRequired_ShouldReturnFalse()
        {
            // Arrange
            string path = "Descargas/";
            string url = "http://172.0.0.1/assetsTree.json";
            string urlWithoutFile = "http://172.0.0.1/";
            Configuration config = new Configuration { Url = url, SavePath = path };

            Archive file = new Archive { Md5 = "fd632797a9adb10d068868f86a2b6951", Name = "keno.html", Required = false, Path = "common/ptf_Terminal/help/en_GB/" };

            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.IsNewFile(file, config.SavePath)).Returns(true);
            downloaderManagerRepo.Setup(a => a.DownloadFileWithRange(urlWithoutFile + file.Path + file.Name, config.SavePath + file.Path + file.Name)).Returns(true);


            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.CopyFileToFolder(file, config);
            // Assert
            Assert.IsFalse(response);
        }
        [Test]
        public void CopyFileToFolder_FileRequiredButNotNew_ShouldReturnFalse()
        {
            // Arrange
            string path = "Descargas/";
            string url = "http://172.0.0.1/assetsTree.json";
            string urlWithoutFile = "http://172.0.0.1/";
            Configuration config = new Configuration { Url = url, SavePath = path };

            Archive file = new Archive { Md5 = "fd632797a9adb10d068868f86a2b6951", Name = "keno.html", Required = true, Path = "common/ptf_Terminal/help/en_GB/" };

            Mock<IDownloaderManagerRepository> downloaderManagerRepo = new Mock<IDownloaderManagerRepository>();
            Mock<ILogger> loggerRepo = new Mock<ILogger>();
            downloaderManagerRepo.Setup(a => a.IsNewFile(file, config.SavePath)).Returns(false);
            downloaderManagerRepo.Setup(a => a.DownloadFileWithRange(urlWithoutFile + file.Path + file.Name, config.SavePath + file.Path + file.Name)).Returns(true);


            DownloaderManager downloaderManager = new DownloaderManager(loggerRepo.Object, downloaderManagerRepo.Object);

            // Act
            bool response = downloaderManager.CopyFileToFolder(file, config);
            // Assert
            Assert.IsFalse(response);
        }
    }
}