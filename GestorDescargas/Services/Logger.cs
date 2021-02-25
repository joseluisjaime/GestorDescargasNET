using GestorDescargas.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDescargas.Services
{
    public class Logger : ILogger
    {

        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
