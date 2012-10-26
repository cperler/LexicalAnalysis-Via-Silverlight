using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace StringTemplate
{
    public class SyntaxAnalyzer
    {
        private Queue<Token> tokens;
        private LexicalAnalyzer lexicalAnalyzer;
        private bool isValid;

        public SyntaxAnalyzer(LexicalAnalyzer lexicalAnalyzer)
        {
            tokens = new Queue<Token>();
            this.lexicalAnalyzer = lexicalAnalyzer;
            isValid = Start();
        }

        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }

        public Queue<Token> Tokens
        {
            get
            {
                return tokens;
            }
        }

        private Token Next()
        {
            Token next = lexicalAnalyzer.Next();
            tokens.Enqueue(next);
            return next;
        }

        private bool Start()
        {
            return E(Next());
        }

        private bool E(Token t)
        {
            Token next = null;
            switch (t.Kind)
            {
                case TokenKind.BreakLine:
                case TokenKind.EndTextbox:
                case TokenKind.EndCenter:
                    next = Next();
                    return CheckEOF(next) || CheckStartElement(next) || CheckEndCenter(next) || CheckEndAnchor(next);
                case TokenKind.StartTextbox:
                    next = Next();
                    return CheckWord(next) || CheckEndTextbox(next);
                case TokenKind.StartCenter:
                    next = Next();
                    return CheckStartTextbox(next) || CheckEndCenter(next) || CheckStartAnchor(next);
                case TokenKind.Word:
                    next = Next();
                    return CheckEndTextbox(next);                
                case TokenKind.StartAnchor:
                    next = Next();
                    return CheckStartTextbox(next) || CheckStartCenter(next);
                case TokenKind.EOF:
                    return true;
                default:
                    return false;
            }
        }

        private bool CheckEndAnchor(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.EndAnchor:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckStartCenter(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.StartCenter:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckStartAnchor(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.StartAnchor:
                    return E(Next());
                default:
                    return false;
            }
        }


        private bool CheckStartTextbox(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.StartTextbox:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckStartElement(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.StartTextbox:
                case TokenKind.BreakLine:
                case TokenKind.StartCenter:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckWord(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.Word:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckEndCenter(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.EndCenter:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckEndTextbox(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.EndTextbox:
                    return E(Next());
                default:
                    return false;
            }
        }

        private bool CheckEOF(Token t)
        {
            switch (t.Kind)
            {
                case TokenKind.EOF:
                    return true;
                default:
                    return false;
            }
        }
    }
}
