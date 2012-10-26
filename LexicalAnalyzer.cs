using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace StringTemplate
{
    public class LexicalAnalyzer
    {
        private const char EOF = (char)0;

        private int column;
        private int pos;
        private string data;
        private int saveCol;
        private int savePos;

        public LexicalAnalyzer(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            this.data = data;

            Reset();
        }

        private void Reset()
        {
            column = 1;
            pos = 0;
        }

        protected char LookAhead(int count)
        {
            if (pos + count >= data.Length)
            {
                return EOF;
            }
            else
            {
                return data[pos + count];
            }
        }

        protected char Consume()
        {
            char ret = data[pos];
            pos++;
            column++;

            return ret;
        }

        protected Token CreateToken(TokenKind kind, string value)
        {
            return new Token(kind, value);
        }

        protected Token CreateToken(TokenKind kind)
        {
            string tokenData = data.Substring(savePos, pos - savePos);
            return new Token(kind, tokenData);
        }

        public Token Next()
        {
            char ch = LookAhead(0);
            switch (ch)
            {
                case EOF:
                    return CreateToken(TokenKind.EOF, string.Empty);
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    {
                        Consume();
                        return Next();
                    }
                case '<':
                    return ReadElement();
                default:
                    return ReadWord();
            }
        }

        private void StartRead()
        {
            saveCol = column;
            savePos = pos;
        }

        protected Token ReadWord()
        {
            StartRead();
            Consume();
            while (true)
            {
                char ch = LookAhead(0);
                if (ch != '<')
                {
                    Consume();
                }
                else
                {
                    break;
                }
            }

            return CreateToken(TokenKind.Word);
        }

        protected Token ReadElement()
        {
            StartRead();
            Consume();
            while (true)
            {
                char ch = LookAhead(0);
                switch (ch)
                {
                    case 'T':
                    case 't':
                        return CreateTextbox();
                    case 'B':
                    case 'b':
                        return CreateBreak();
                    case 'C':
                    case 'c':
                        return CreateCenter();
                    case 'A':
                    case 'a':
                        return CreateAnchor();
                    case '/':
                        return CreateEnd();
                    default:
                        return CreateToken(TokenKind.Unknown);
                }
            }
        }        

        private string Lookup()
        {
            string attempt = string.Empty;

            while (true)
            {
                char ch = LookAhead(0);
                if (ch != '>')
                {
                    Consume();
                    attempt += ch;
                }
                else
                {
                    Consume();
                    break;
                }
            }
            return attempt;
        }

        protected Token CreateEnd()
        {
            Consume();

            while (true)
            {
                char ch = LookAhead(0);
                switch (ch)
                {
                    case 'T':
                    case 't':
                        return CreateEndTextbox();
                    case 'C':
                    case 'c':
                        return CreateEndCenter();
                    case 'A':
                    case 'a':
                        return CreateEndAnchor();
                    default:
                        return CreateToken(TokenKind.Unknown);
                }

            }
        }

        protected Token CreateAnchor()
        {
            return CheckForToken("a", TokenKind.StartAnchor);
        }

        protected Token CreateEndAnchor()
        {
            return CheckForToken("a", TokenKind.EndAnchor);
        }

        protected Token CreateCenter()
        {
            return CheckForToken("center", TokenKind.StartCenter);
        }

        protected Token CreateEndCenter()
        {
            return CheckForToken("center", TokenKind.EndCenter);            
        }

        protected Token CreateTextbox()
        {
            return CheckForToken("textblock", TokenKind.StartTextbox);
        }

        protected Token CreateEndTextbox()
        {
            return CheckForToken("textblock", TokenKind.EndTextbox);
        }

        protected Token CreateBreak()
        {
            return CheckForToken("br", TokenKind.BreakLine);            
        }

        private Token CheckForToken(string lookFor, TokenKind kind)
        {
            string tryCenter = Lookup();

            if (tryCenter.StartsWith(lookFor, StringComparison.CurrentCultureIgnoreCase))
            {
                return CreateToken(kind);
            }
            else
            {
                return CreateToken(TokenKind.Unknown);
            }
        }
    }
}