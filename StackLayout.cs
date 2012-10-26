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
    public class StackLayout : Canvas, IClippable
    {
        public ExpansionMode ExpansionMode { get; set; }

        public StackLayout() : base()
        {
            Loaded += new EventHandler(HandleLoaded);
        }

        private void HandleLoaded(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        public override void SetValue<T>(DependencyProperty property, T obj)
        {
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged(this, new DependencyArgs(property, obj));
            }

            base.SetValue<T>(property, obj);
        }

        private void UpdateLayout()
        {
            if (Children.Count > 0)
            {                
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i] is LayoutPanel)
                    {
                        LayoutPanel panel = (LayoutPanel)Children[i];

                        panel.SetValue<double>(Canvas.LeftProperty, 0);
                        if (i == 0)
                        {
                            panel.SetValue<double>(Canvas.TopProperty, 0);
                        }
                        else
                        {
                            LayoutPanel prevPanel = (LayoutPanel)Children[i - 1];
                            SizePanel(panel, prevPanel);
                        }

                        if (i == Children.Count - 1)
                        {
                            LayoutPanel prevPanel = (LayoutPanel)Children[i - 1];
                            ExpandLastPanel(panel, prevPanel);
                        }
                    }
                }
            }
        }

        private static void SizePanel(LayoutPanel panel, LayoutPanel prevPanel)
        {
            double prevTop = (double)prevPanel.GetValue(Canvas.TopProperty);
            double prevHeight = (double)prevPanel.ActualHeight;
            panel.SetValue<double>(Canvas.TopProperty, prevTop + prevHeight);
        }

        private void ExpandLastPanel(LayoutPanel panel, LayoutPanel prevPanel)
        {
            double lastTop = (double)panel.GetValue(Canvas.TopProperty);
            double lastHeight = prevPanel.Height;
            double top = (double)GetValue(Canvas.TopProperty);
            if (lastTop + lastHeight < top + Height)
            {
                panel.ExpansionMode = ExpansionMode;

                if (ExpansionMode == ExpansionMode.Normal)
                {
                    panel.Height = top + Height - lastTop;
                }
            }
        }

        public event EventHandler<DependencyArgs> OnPropertyChanged;
    }
}
