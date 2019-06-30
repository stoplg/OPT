using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace translator
{
    struct Token
    {
        public int code;
        public int line;
        public int column;
        public string strTok;

        public Token(int code, int line, int column, string strTok)
        {
            this.code = code;
            this.line = line;
            this.column = column;
            this.strTok = strTok;
        }
    }
    enum symbType
    {
        digit,
        letter,
        dm,
        ws,
        begComm,
        twoSymbDmPt
    }

    class Lexer
    {
        public List<Token> tokens;
        public List<string> errors;
        public Dictionary<string, int> identifiersTable;
        public Dictionary<string, int> keywordsTable;
        public Dictionary<string, int> integersTable;
        public Dictionary<string, int> twoSymbDmTable;
        public Dictionary<int, symbType> alphabet;
        private const int begIdnt = 501;
        private const int begInt = 1001;
        private string filePath;

        public Lexer(string filePath)
        {
            tokens = new List<Token>();
            errors = new List<string>();
            InitTables();
            this.filePath = filePath;
            MainReader();
        }

        private void MainReader()
        {
            StreamReader reader;
            try
            {
                reader = new StreamReader(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                return;
            }
            int curSymb = reader.Read(), curLine = 1, curColumn = 1;
            while (curSymb != -1)
            {
                if (!alphabet.ContainsKey((char)curSymb))
                {
                    errors.Add($"Forbidden symbol: {(char)curSymb} at position ({curLine},{curColumn}).");
                    curColumn++;
                    curSymb = reader.Read();
                    continue;
                }
                switch (alphabet[(char)curSymb])
                {
                    case symbType.digit:
                        ReadInteger(ref curSymb, ref curColumn, curLine, reader);
                        continue;
                    case symbType.begComm:
                        if (((char)(curSymb = reader.Read())) == '*')
                        {
                            curColumn += 2;
                            ReadComment(ref curColumn, ref curLine, reader);
                        }
                        else
                        {
                            errors.Add($"Forbidden symbol: ( at position ({curLine},{curColumn}).");
                            curColumn++;
                            continue;
                        }
                        break;
                    case symbType.letter:
                        ReadString(ref curSymb, ref curColumn, curLine, reader);
                        continue;
                    case symbType.twoSymbDmPt:
                        ReadTwoSymbDm(ref curSymb, ref curColumn, curLine, reader);
                        continue;
                    case symbType.dm:
                        tokens.Add(new Token(curSymb, curLine, curColumn, ((char)curSymb).ToString()));
                        curColumn++;
                        break;
                    case symbType.ws:
                        if (curSymb == 10)
                        {
                            curLine++;
                            curColumn = 1;
                        }
                        else
                        {
                            curColumn++;
                        }
                        break;
                }

                curSymb = reader.Read();
            }
            reader.Close();
        }
        private void ReadComment(ref int curColumn, ref int curLine, StreamReader reader)
        {
            int comBegLine = curLine, comBegColumn = curColumn - 2;
            int curSymb = reader.Read();
            bool unclosedComment = true; 
            while (curSymb != -1)
            {
                curColumn++;
                if (((char)curSymb) == '*')
                {
                    while (((char)(curSymb = reader.Read())) == '*')
                    {
                        curColumn++;
                    }
                    curColumn++;
                    if (((char)curSymb) == ')')
                    {
                        unclosedComment = false;
                        break;
                    }
                }
                if (curSymb == 10)
                {
                    curLine++;
                    curColumn = 1;
                }
                curSymb = reader.Read();
            }
            if (unclosedComment)
            {
                errors.Add($"Unclosed comment begins at ({comBegLine} , {comBegColumn}).");
            }
        }
        private void ReadTwoSymbDm(ref int firstSymb, ref int curColumn, int curLine, StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)firstSymb);
            bool correctDelim = true;
            firstSymb = reader.Read();
            if (Char.IsLetterOrDigit((char)firstSymb))
            {
                correctDelim = false;
            }
            builder.Append((char) firstSymb);
            string tokenString = builder.ToString();
            if (correctDelim)
            {
                if (twoSymbDmTable.ContainsKey(tokenString))
                {
                    tokens.Add(new Token(twoSymbDmTable[tokenString], curLine, curColumn, tokenString));
                }
                else
                {
                    errors.Add($"Forbidden delimiter: at position ({curLine},{curColumn}).");
                }
            }
            curColumn += tokenString.Length;
        }
        private void ReadInteger(ref int firstSymb, ref int curColumn, int curLine, StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)firstSymb);
            bool correctInteger = true;
            while (Char.IsLetterOrDigit((char)(firstSymb = reader.Read())))
            {
                if (Char.IsLetter((char)firstSymb))
                {
                    correctInteger = false;
                }
                builder.Append((char)firstSymb);
            }
            string tokenString = builder.ToString();
            if (correctInteger)
            {
                if (integersTable.ContainsKey(tokenString))
                {
                    tokens.Add(new Token(integersTable[tokenString], curLine, curColumn, tokenString));
                }
                else
                {
                    integersTable.Add(tokenString, begInt + integersTable.Count);
                    tokens.Add(new Token(integersTable[tokenString], curLine, curColumn, tokenString));
                }
            }
            else{
                errors.Add($"Letter can't be part of integer: at position ({curLine},{curColumn}).");
            }
            curColumn += tokenString.Length;
        }
        private void ReadString(ref int firstSymb, ref int curColumn, int curLine, StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)firstSymb);
            while(Char.IsLetterOrDigit((char)(firstSymb = reader.Read())))
            {
                builder.Append((char)firstSymb);
            }
            string tokenString = builder.ToString();
            if (keywordsTable.ContainsKey(tokenString.ToUpper()))
            {
                tokens.Add(new Token(keywordsTable[tokenString.ToUpper()], curLine, curColumn, tokenString));
            }
            else if (identifiersTable.ContainsKey(tokenString))
            {
                tokens.Add(new Token(identifiersTable[tokenString], curLine, curColumn, tokenString));
            }
            else
            {
                identifiersTable.Add(tokenString, begIdnt + identifiersTable.Count);
                tokens.Add(new Token(identifiersTable[tokenString], curLine, curColumn, tokenString));
            }
            curColumn += tokenString.Length;
        }
        private void InitTables()
        {
            identifiersTable = new Dictionary<string, int>();
            integersTable = new Dictionary<string, int>();
            twoSymbDmTable = new Dictionary<string, int>
            {
                {"..",301 },
            };
            keywordsTable = new Dictionary<string, int>
            {
                    {"PROGRAM", 401},
                    {"VAR", 402},
                    {"SIGNAL", 403},
                    {"COMPLEX", 404},
                    {"INTEGER", 405},
                    {"FLOAT", 406},
                    {"BLOCKFLOAT", 407},
                    {"EXT", 408},
                    {"BEGIN", 409},
                    {"END", 410},
            };
            alphabet = new Dictionary<int, symbType>();
            for (int i = 'a'; i <= 'z'; i++)
            {
                alphabet.Add(i, symbType.letter);
            }
            for (int i = 'A'; i <= 'Z'; i++)
            {
                alphabet.Add(i, symbType.letter);
            }
            for (int i = '0'; i <= '9'; i++)
            {
                alphabet.Add(i, symbType.digit);
            }
            for (int i = 8; i <= 13; i++)
            {
                alphabet.Add(i, symbType.ws);
            }
            alphabet.Add(32, symbType.ws);
            alphabet.Add(';', symbType.dm);
            alphabet.Add(':', symbType.dm);
            alphabet.Add('[', symbType.dm);
            alphabet.Add(']', symbType.dm);
            alphabet.Add('.',symbType.twoSymbDmPt);
            alphabet.Add('(', symbType.begComm);
        }
    }
}
