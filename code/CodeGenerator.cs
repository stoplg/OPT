using System;
using System.Collections.Generic;
namespace translator
{
    class CodeGenerator
    {
        private Lexer lexer;
        public List<Token> token;
        public List<string> AsmCode;
        public CodeGenerator(List<Token> tokens)
        {

            token = tokens;
        }
        public void generateCode(List<Token> token)
        {
            this.token = token;
            for (int i=0;i<token.Count;i++)
            {
                switch (token[i].code)
                {
                    case 401:
                        programStart(i);
                        break;
                    case 402:
                        variableDeclaration(i);
                        break;
                    case 409:
                        codeStart(i);
                        break;
                    case 410:
                        programEnd(i);
                        break;
                }
            }
        }
        private void variableDeclaration(int i)
        {
            string varName = token[i + 1].strTok;
            string varType;
            switch (token[i + 3].strTok)
            {
                case "[":
                    varType = " DQ dup(" + token[i + 6].strTok + "-" + token[i + 4].strTok + ")";
                    break;
                default:
                    varType = " DQ   ?";
                    break;
            }
            Console.WriteLine(varName+varType);
        }
        private void programEnd(int i)
        {
            Console.WriteLine("code ends\n");
            Console.WriteLine("end\n");
        }
        private void programStart(int i)
        {
            Console.WriteLine($";{token[i+1].strTok}\n");
            Console.WriteLine(".386\n data segment\n");
        } 
        private void codeStart(int i)
        {
            Console.WriteLine("data ends\n");
            Console.WriteLine("code segment\n");
            Console.WriteLine("assume cs:code,ds:data");
        }
    }
}

