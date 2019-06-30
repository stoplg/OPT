using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace translator
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "test.txt";
            Lexer lexer = new Lexer(path);
            Printer printer = new Printer();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("########################## LEXER RESULT:");
            Console.WriteLine("**************               ALPAHABET         **************");
            printer.PrintAlphabet(lexer.alphabet);
            Console.WriteLine();
            printer.PrintTable(lexer.keywordsTable);
            Console.WriteLine();
            Console.WriteLine("**************             IDENTIFIERS         **************");
            printer.PrintTable(lexer.identifiersTable);
            Console.WriteLine();
            Console.WriteLine("**************              CONSTANTS          **************");
            printer.PrintTable(lexer.integersTable);
            Console.WriteLine();
            Console.WriteLine("**************               TOKENS            **************");
            printer.PrintTokens(lexer.tokens);
            Console.WriteLine();
            Console.WriteLine("**************            LEXICAL ERRORS       **************");
            printer.PrintErrors(lexer.errors);
            if (lexer.errors.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("########################## PARSER RESULT:");
                Parser parser = new Parser(lexer);
                parser.GetTreeRoot().PrintTree();
                Console.WriteLine("**************            PARSE ERRORS       **************");
                printer.PrintErrors(parser.errors);
                if (parser.errors.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("########################## CODEGEN RESULT:");
                    Lexer _lexer = new Lexer(path);
                    List<Token> _tokens = _lexer.tokens;
                    CodeGenerator generator = new CodeGenerator(_tokens);
                    generator.generateCode(_tokens);
                }
            }
            Console.ReadLine();
        }
    }
}