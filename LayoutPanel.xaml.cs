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
    public enum ExpansionMode
    {
        Normal,
        Actual
    }

    public class LayoutPanel : ControlBase, IClippable
    {
        private static int count = 0;
        private bool suspendUpdates = false;
        private ExpansionMode expansionMode = ExpansionMode.Normal;
        private AutoClip clip;

        private Canvas parent;

        public LayoutPanel() : base()
        {
            parent = (Canvas)FindName("layoutPanel");
            parent.SetValue<string>(Canvas.NameProperty, parent.Name + (count++));

            Loaded += new EventHandler(HandleLoaded);
        }

        private void HandleLoaded(object sender, EventArgs e)
        {
            clip = new AutoClip(this);
            UpdateChildrenLayout();
        }

        private void UpdateChildrenLayout()
        {
            suspendUpdates = true;
            UpdateChildren();
            suspendUpdates = false;
        }

        private void UpdateChildren()
        {
            foreach (FrameworkElement child in Children)
            {
                child.Width = Width;
                child.Height = Height;
                child.SetValue<double>(Canvas.LeftProperty, 0);
                //child.SetValue<double>(Canvas.TopProperty, 0);
                
                if (clip != null)
                {
                    clip.Clip(child);
                }

                try
                {
                    child.SetValue<int>(TextBlock.TextWrappingProperty, (int)TextWrapping.Wrap);
                }
                catch (Exception) { }
            }
        }

        public override void SetValue<T>(DependencyProperty property, T obj)
        {
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged(this, new DependencyArgs(property, obj));
            }
            
            base.SetValue<T>(property, obj);

            if (!suspendUpdates)
            {
                UpdateChildrenLayout();
            }
        }

        public virtual new double Height
        {
            get
            {
                if (expansionMode == ExpansionMode.Normal)
                {
                    return base.Height;
                }
                else
                {
                    return ActualHeight;
                }
            }
            set
            {
                base.Height = value;
            }
        }

        public double ActualHeight
        {
            get
            {
                double max = 0;

                foreach (FrameworkElement child in Children)
                {
                    try
                    {
                        max = Math.Max(max, (double)child.GetValue(TextBlock.ActualHeightProperty));
                    }
                    catch (Exception) { }
                }

                return max;
            }
        }

        public ExpansionMode ExpansionMode
        {
            get
            {
                return expansionMode;
            }
            set
            {
                expansionMode = value;
                UpdateChildrenLayout();
            }
        }

        public event EventHandler<DependencyArgs> OnPropertyChanged;

        protected override string ResourceName
        {
            get { return "LayoutPanel.xaml"; }
        }
    }
}