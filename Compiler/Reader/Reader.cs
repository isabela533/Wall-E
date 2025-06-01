namespace Compiler.Reader;

public interface IReader
{
    string ReadFromEditor(string text);
    string ReadFromFile(string filePath);
}
