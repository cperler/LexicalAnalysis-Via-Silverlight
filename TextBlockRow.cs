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
    public enum Alignment
    {
        Left,
        Center,
        Right,
        Full
    }

    public class TextBlockRow : Control
    {
        public List<LinkableTextBlock> Blocks { get; set; }
        public bool AutoSpace { get; set; }
        public Alignment Aligned { get; set; }
        public double AutoSpaceLength { get; set; }
        private double width;
        private double height;

        public TextBlockRow() : base()
        {
            Blocks = new List<LinkableTextBlock>();
            AutoSpace = true;
            Aligned = Alignment.Left;
            AutoSpaceLength = 5;

            Loaded += new EventHandler(MultiTextBlock_Loaded);
        }

        public override void SetValue<T>(DependencyProperty property, T obj)
        {
            if (obj is double)
            {
                double value = Convert.ToDouble(obj);
                if (property == Canvas.LeftProperty)
                {
                    AnchorLeft(value);
                }
                else if (property == Canvas.TopProperty)
                {
                    AnchorTop(value);
                }
            }
            base.SetValue<T>(property, obj);
        }

        private void AnchorLeft(double left)
        {
            if (Blocks.Count > 0)
            {
                Blocks[0].SetValue<double>(Canvas.LeftProperty, left);

                for (int i = 1; i < Blocks.Count; i++)
                {
                    LinkableTextBlock prev = Blocks[i - 1];
                    LinkableTextBlock cur = Blocks[i];

                    double prevLeft = (double)prev.GetValue(Canvas.LeftProperty);
                    double prevWidth = prev.ActualWidth;

                    if (AutoSpace)
                    {
                        cur.SetValue<double>(Canvas.LeftProperty, prevLeft + prevWidth + AutoSpaceLength);
                    }
                    else
                    {
                        cur.SetValue<double>(Canvas.LeftProperty, prevLeft + prevWidth);
                    }
                }
            }
        }

        private void AnchorTop(double top)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].SetValue<double>(Canvas.TopProperty, top);
            }
        }

        private void MultiTextBlock_Loaded(object sender, EventArgs e)
        {
            AnchorLeft((double)GetValue(Canvas.LeftProperty));
            AnchorTop((double)GetValue(Canvas.TopProperty));
            Width = Width;
            Height = Height;

            foreach (LinkableTextBlock ltb in Blocks)
            {
                ((Canvas)Parent).Children.Add(ltb);
            }
        }

        public new virtual double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                double count = width;
                for (int i = 0; i < Blocks.Count; i++)
                {
                    if (count < Blocks[i].ActualWidth)
                    {
                        Blocks[i].Width = Math.Max(0, count);
                    }

                    count -= Blocks[i].ActualWidth;                    
                }
            }
        }

        public new virtual double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                for (int i = 0; i < Blocks.Count; i++)
                {
                    Blocks[i].Height = height;
                }
            }
        }
    }
}