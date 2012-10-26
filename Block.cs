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
using System.Windows.Markup;
using System.Diagnostics;

namespace StringTemplate
{
    public static class BlockLoader
    {
        public static void Load(string block, Canvas canvas, Point upperLeftCorner)
        {
            LexicalAnalyzer la = new LexicalAnalyzer(block);
            SyntaxAnalyzer sa = new SyntaxAnalyzer(la);
            if (sa.IsValid)
            {
                BlockCreator bc = new BlockCreator(sa, upperLeftCorner);

                foreach (TextBlock tb in bc.Blocks)
                {
                    canvas.Children.Add(tb);
                }
            }
        }
    }
}