using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Compiler.Reader;

public class EditorFileReader : IReader
{
    public string ReadFromEditor(string text)
    {
        return text; // devuelve el texto tal cual fue ingresado 
    }

    public string ReadFromFile(string filePath)
    {
        return File.ReadAllText(filePath); // logica para leer archivos 
    }
}