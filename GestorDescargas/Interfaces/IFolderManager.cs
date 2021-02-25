using GestorDescargas.Models;
using System.Collections.Generic;
using System.IO;

namespace GestorDescargas.Interfaces
{
    internal interface IFolderManager
    {
        void CreateFolders(Folder folder, Configuration config);

    }
}