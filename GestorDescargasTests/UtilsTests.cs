using NUnit.Framework;
using GestorDescargas.Services;

namespace GestorDescargasTests
{
    public class UtilsTests
    {

        [Test]
        public void IsValidURL_ValidUrl_ShouldReturnTrue()
        {
            // Arrange
            var stringUrl = "http://google.com";

            // Act
            var response = Utils.IsValidURL(stringUrl);

            // Assert
            Assert.IsTrue(response);
        }

        [Test]
        public void IsValidURL_InvalidUrl_ShouldReturnFalse()
        {
            // Arrange
            var stringUrl = "google.com";

            // Act
            var response = Utils.IsValidURL(stringUrl);

            // Assert
            Assert.IsFalse(response);
        }

        [Test]
        [TestCase("common[0].ptf_Terminal[1].help", "common/ptf_Terminal/help")]
        [TestCase("common[0].ptf_Terminal","common/ptf_Terminal")]
        public void FormatFolderPath_StringPath_ShouldReturnStringPathFormated(string path, string pathExpected)
        {
            // Arrange

            // Act
            var response = Utils.FormatFolderPath(path);

            // Assert
            Assert.AreEqual(response, pathExpected);
        }

        [Test]
        [TestCase("common[0].ptf_Terminal[1].help/md5", "common/ptf_Terminal/help/")]
        [TestCase("common[0].ptf_Terminal/md5", "common/ptf_Terminal/")]
        public void FormatFilePath_StringPath_ShouldReturnStringFilePathFormated(string path, string pathExpected)
        {
            // Arrange

            // Act
            var response = Utils.FormatFilePath(path);

            // Assert
            Assert.AreEqual(response, pathExpected);
        }

        [Test]
        [TestCase("http://myserver.com/file.json", "http://myserver.com/")]
        [TestCase("http://myserver.com/folder1/folder2/file.json", "http://myserver.com/folder1/folder2/")]
        public void UrlWithoutFile_StringUrl_ShouldReturnStringUrlFormated(string path, string pathExpected)
        {
            // Arrange

            // Act
            var response = Utils.UrlWithoutFile(path);

            // Assert
            Assert.AreEqual(response, pathExpected);
        }
    }
}