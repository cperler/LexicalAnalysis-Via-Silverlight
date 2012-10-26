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
using System.Collections.ObjectModel;

namespace StringTemplate
{
    public class BlockCreator
    {
        private SyntaxAnalyzer syntaxAnalyzer = null;
        private Point upperLeft;
        private double x = 0;
        private double y = 0;        
        private List<TextBlock> blocks = null;
        private Stack<Token> tokens = null;
        private List<List<TextBlock>> rows = null;
        private List<BlockSpan> toCenter = null;
        private bool linked = false;

        public BlockCreator(SyntaxAnalyzer syntaxAnalyzer, Point upperLeft)
        {
            this.syntaxAnalyzer = syntaxAnalyzer;
            this.upperLeft = upperLeft;
            x = upperLeft.X;
            y = 0;
            blocks = new List<TextBlock>();
            tokens = new Stack<Token>();

            rows = new List<List<TextBlock>>();
            rows.Add(new List<TextBlock>());

            toCenter = new List<BlockSpan>();

            Start();
            CenterBlocks();
        }

        private void Start()
        {
            Token t = Next();
            while (t.Kind != TokenKind.EOF)
            {
                switch (t.Kind)
                {
                    case TokenKind.StartTextbox:
                    case TokenKind.Word:                    
                        tokens.Push(t);
                        break;
                    case TokenKind.BreakLine:                        
                        x = upperLeft.X;
                        rows.Add(new List<TextBlock>());
                        tokens.Push(t);
                        break;
                    case TokenKind.EndTextbox:
                        tokens.Push(t);
                        CreateTextBox();
                        break;
                    case TokenKind.StartCenter:
                        StartCenter();
                        break;
                    case TokenKind.EndCenter:
                        EndCenter();
                        break;
                    case TokenKind.StartAnchor:
                        linked = true;
                        break;
                    case TokenKind.EndAnchor:
                        linked = false;
                        break;
                    default:
                        break;
                }

                t = Next();
            }
        }

        private void StartCenter()
        {
            toCenter.Add(new BlockSpan(
                new BlockReference(rows.Count - 1, rows[rows.Count - 1].Count),
                new BlockReference(rows.Count - 1, rows[rows.Count - 1].Count)));
        }

        private void EndCenter()
        {
            BlockSpan last = toCenter[toCenter.Count - 1];
            last.End = new BlockReference(rows.Count - 1, rows[rows.Count - 1].Count);
        }

        private void CreateTextBox()
        {
            string xaml = string.Empty;
            
            Token t = tokens.Pop();
            while (t.Kind != TokenKind.StartTextbox)
            {
                xaml = t.Value + xaml;
                t = tokens.Pop();
            }
            xaml = t.Value + xaml;

            TextBlock tb = (TextBlock)XamlReader.Load(xaml);
            PeekForBreak(tb);

            tb.SetValue<double>(Canvas.LeftProperty, x);
            tb.SetValue<double>(Canvas.TopProperty, y);
            rows[rows.Count - 1].Add(tb);
            blocks.Add(tb);

            x += tb.ActualWidth;

            if (linked)
            {
                tb.MouseEnter += new MouseEventHandler(OnMouseEnter);
                tb.MouseLeave += new EventHandler(OnMouseLeave);
                tb.MouseLeftButtonDown += new MouseEventHandler(OnMouseClick);
                tb.MouseLeftButtonUp += new MouseEventHandler(OnMouseRelease);
            }
        }

        private void OnMouseRelease(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            Canvas canvas = (Canvas)tb.Parent;
            if (canvas.FindName(tb.Name + "_highlight") != null)
            {
                canvas.Children.Remove((Visual)canvas.FindName(((TextBlock)sender).Name + "_highlight"));
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            
            Rectangle bg = new Rectangle();
            bg.SetValue<string>(Canvas.NameProperty, tb.Name + "_highlight");
            bg.Width = tb.ActualWidth;
            bg.Height = tb.ActualHeight;
            bg.SetValue<double>(Canvas.TopProperty, (double)tb.GetValue(Canvas.TopProperty));
            bg.SetValue<double>(Canvas.LeftProperty, (double)tb.GetValue(Canvas.LeftProperty));
            bg.SetValue<int>(Canvas.ZIndexProperty, (int)tb.GetValue(Canvas.ZIndexProperty) - 1);
            bg.Fill = new SolidColorBrush(Colors.LightGray);

            ((Canvas)tb.Parent).Children.Add(bg);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            string[] details = tb.Tag.Split(';');
            tb.Foreground = new SolidColorBrush(ColorUtil.HexToColor(details[0]));
            tb.TextDecorations = (TextDecorations)Enum.Parse(typeof(TextDecorations), details[1], true);

            Canvas canvas = (Canvas)tb.Parent;
            if( canvas.FindName(tb.Name + "_highlight") != null)
            {
                canvas.Children.Remove((Visual)canvas.FindName(((TextBlock)sender).Name + "_highlight"));
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Tag = ((SolidColorBrush)tb.Foreground).Color.ToString() + ";" + tb.TextDecorations.ToString();
            tb.Foreground = new SolidColorBrush(Colors.Blue);
            tb.TextDecorations = TextDecorations.Underline;
        }

        private void CenterBlocks()
        {
            if (toCenter.Count > 0)
            {
                double maxWidth = double.MinValue;                
                foreach (List<TextBlock> row in rows)
                {
                    double rowWidth = 0;
                    foreach (TextBlock tb in row)
                    {
                        rowWidth += tb.ActualWidth;
                    }
                    maxWidth = Math.Max(maxWidth, rowWidth);
                }

                foreach (BlockSpan span in toCenter)
                {
                    for (int i = span.Start.Row; i <= span.End.Row; i++)
                    {
                        int start = (span.Start.Row == span.End.Row) ? span.Start.Col : 0;
                        int end = (span.Start.Row == span.End.Row) ? span.End.Col : rows[i].Count;

                        double rowWidth = 0;
                        for (int j = start; j < end; j++)
                        {
                            TextBlock tb = rows[i][j];
                            rowWidth += tb.ActualWidth;
                        }

                        double moveOver = (maxWidth - rowWidth) / 2.0;

                        for (int j = start; j < end; j++)
                        {
                            TextBlock tb = rows[i][j];
                            tb.SetValue<double>(Canvas.LeftProperty, (double)tb.GetValue(Canvas.LeftProperty) + moveOver);
                        }
                    }
                }
            }
        }

        private void PeekForBreak(TextBlock tb)
        {
            if (tokens.Count > 0 && tokens.Peek().Kind == TokenKind.BreakLine)
            {
                tokens.Pop();
                if (blocks.Count > 0)
                {
                    y += blocks[blocks.Count - 1].ActualHeight;
                    y -= tb.FontSize;
                }
            }
        }

        private Token Next()
        {
            return syntaxAnalyzer.Tokens.Dequeue();
        }

        public IList<TextBlock> Blocks
        {
            get
            {
                return new ReadOnlyCollection<TextBlock>(blocks);
            }
        }

        private class BlockReference
        {
            public int Row { get; set; }
            public int Col { get; set; }

            public BlockReference(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }

        private class BlockSpan
        {
            public BlockReference Start { get; set; }
            public BlockReference End { get; set; }

            public BlockSpan(BlockReference start, BlockReference end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
