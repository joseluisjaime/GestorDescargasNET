using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDescargas.Models
{
    public class Archive
    {
        public string Md5 { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public string Path { get; set; }

    }
}
