using GestorDescargas.Interfaces;
using GestorDescargas.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GestorDescargas.Services
{
    class FolderManager : IFolderManager
    {

        public void CreateFolders(Folder folder, Configuration config)
        {
            try
            {
                Directory.CreateDirectory(config.SavePath + folder.Path);

            }
            catch (Exception e)
            {
                  throw e;
            }

        }

    }
}
