using Compiler.Model;
using Compiler.Parser;
using Compiler.Reader;
using Compiler.Tokenizador;

namespace Test;

public class Program
{
    static string Dir = @"content";
    public static void Main(string[] args)
    {
        // Configuracion por defecto
        var fileName = "test.pw";
        var path = GetPath(fileName);
        var parser = new Parser();
        var context = new Context();

        // Cosas del proyecto Original
        var reader = new StreamReader(path);
        string code = reader.ReadToEnd();
        Token[] tokens = Tokenizador.Tokenizar(code);
        var ast = parser.Parse(tokens);
        ast.Accept(context);
    }

    public static string GetPath(string fileName)
    {
        return Path.Combine(Dir, fileName);
    }
}
