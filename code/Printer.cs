using System;
using System.Collections.Generic;

namespace translator
{
    class Printer
    {
        public void PrintTokens(List<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                Console.WriteLine($"{token.code} ({token.line} , {token.column}) - {token.strTok}");
            }
        }

        public void PrintTable(Dictionary<string, int> table)
        {
            foreach (KeyValuePair<string, int> pair in table)
            {
                Console.WriteLine($"{pair.Value} - {pair.Key}");
            }
        }

        public void PrintErrors(List<string> errors)
        {
           
            if (errors.Count > 0)
            {
                foreach (string error in errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Console.WriteLine("                          NO  ERRORS");
            }
        }

        public void PrintAlphabet(Dictionary<int, symbType> alphabet)
        {
            foreach (KeyValuePair<int, symbType> pair in alphabet)
            {
                Console.WriteLine($"{(char)pair.Key} - {pair.Value.ToString()}");
            }
        }
    }
}
