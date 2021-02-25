using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDescargas.Models
{
    public class StructureFolderFile
    {
        public List<Archive> ListArchives { get; set; }
        public List<Folder> ListFolders { get; set; }
    }
}
