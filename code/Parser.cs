using System;
using System.CodeDom;
using System.Collections.Generic;
namespace translator
{
    class Parser
    {
        private int curLine;
        Lexer lexer;
        TreeNode root;
        private List<Token> tokens;
        public List<string> errors;

        public TreeNode GetTreeRoot() => root;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            errors = new List<string>();
            tokens = lexer.tokens;
            SignalProgram();
        }

        private void SignalProgram()
        {
            curLine = 1;
            root = new TreeNode("<signal-program>");
            Program(root);

        }

        private void Program(TreeNode parent)
        {
            TreeNode curNode = parent.AddChild("<program.");
            if (tokens.Count != 0 && tokens[0].strTok.ToUpper() == "PROGRAM")
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"'PROGRAM' was expected at line {curLine}");
            }

            ProcedureIdentifier(curNode);
            if (tokens.Count != 0 && tokens[0].strTok == ";")
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"';' was expected at line {curLine}");
            }

            Block(curNode);
        }

        private void Block(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<block>");
            Declarations(curNode);
            if (tokens.Count != 0 && tokens[0].strTok.ToUpper() == "BEGIN")
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"'BEGIN' was expected at line {curLine}");
            }
            StatementsList(curNode);
            if (tokens.Count != 0 && tokens[0].strTok.ToUpper() == "END")
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"'END' was expected at line {curLine}");
            }
        }

        private void Declarations(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<declarations>");
            VariableDeclaration(curNode);
        }

        private void VariableDeclaration(TreeNode parent)
        {
            
            if (tokens.Count != 0 && tokens[0].strTok.ToUpper() == "VAR")
            {
                TreeNode curNode = parent.AddChild("<variable-declaration>");
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
                DeclarationsList(curNode);
                VariableDeclaration(curNode);
            }
        }

        private void DeclarationsList(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<declarations-list>");
            Declaration(curNode);
        }
        
        private void Declaration(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<declaration>");
            VariableIdentifier(curNode);
        }

        private void StatementsList(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<statements-list>");
            if (tokens[0].strTok.ToUpper()!="END")
            {
                errors.Add("Statements list must be empty");
            }
            else
            {
                curNode.AddChild("<empty>");
            }
        }
        
        private void VariableIdentifier(TreeNode parent)
        {
            TreeNode curNode = parent.AddChild("<variable-identifier>");
            Identifier(curNode);
            if (tokens.Count != 0 && tokens[0].strTok == ":")
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
                AttributesList(curNode);
            }
            
        }

        private void AttributesList(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<attributes-list>");
            Attribute(curNode);
        }

        private void Attribute(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<attribute>");
            if (tokens.Count != 0)
            {
                if (tokens[0].code>=403&&tokens[0].code<=408)
                {
                    curNode.AddChild(tokens[0].strTok, tokens[0].code);
                    tokens.RemoveAt(0);
                }
                else if (tokens[0].strTok == "[")
                {
                    curNode.AddChild(tokens[0].strTok, tokens[0].code);
                    tokens.RemoveAt(0);
                    RangesList(curNode);
                }
                else
                {
                    errors.Add($"Forbidden attribute at line {curLine}");
                }
            }
        }

        private void RangesList(TreeNode parent)
        {
            TreeNode curNode = parent.AddChild("<ranges-list>");
            Range(curNode);
        }

        private void Range(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<range>");
            UnsignedInteger(curNode);
            if (tokens[0].strTok == "..")
            {
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"'..' was expected at line {curLine}");
            }
            UnsignedInteger(curNode);
            if (tokens[0].strTok == "]")
            {
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"']' was expected at line {curLine}");
            }
        }

        private void ProcedureIdentifier(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<procedure-identifier>");
            Identifier(curNode);
        }

        private void Identifier(TreeNode parent)
        {
            TreeNode curNode = parent.AddChild("<identifier>");
            if (tokens.Count != 0 && tokens[0].code > 500 && tokens[0].code < 1000)
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"Identifier was expected at line {curLine}");
            }
        }

        private void UnsignedInteger(TreeNode parent)
        {
            curLine = tokens[0].line;
            TreeNode curNode = parent.AddChild("<unsigned-integer>");
            if (tokens.Count != 0 && tokens[0].code > 1000 && tokens[0].code < 2000)
            {
                curLine = tokens[0].line;
                curNode.AddChild(tokens[0].strTok, tokens[0].code);
                tokens.RemoveAt(0);
            }
            else
            {
                errors.Add($"Unsigned integer was expected at line {curLine}");
            }
        }

    }
}
