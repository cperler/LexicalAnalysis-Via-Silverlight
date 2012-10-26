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
    public enum TokenKind
    {
        Unknown,
        StartTextbox,
        BreakLine,
        EndTextbox,
        StartCenter,
        EndCenter,
        StartAnchor,
        EndAnchor,
        Word,
        EOF
    }

    public class Token
    {
        private string value;
        private TokenKind kind;

        public Token(TokenKind kind, string value)
        {
            this.kind = kind;
            this.value = value;
        }

        public TokenKind Kind
        {
            get { return this.kind; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public override string ToString()
        {
            return kind + " = " + value;
        }
    }

}
