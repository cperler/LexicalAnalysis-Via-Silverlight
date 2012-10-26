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
    public class DependencyArgs : EventArgs
    {
        public DependencyProperty Property { get; set; }
        public object NewValue { get; set; }

        public DependencyArgs(DependencyProperty prop, object newValue)
        {
            Property = prop;
            NewValue = newValue;
        }
    }

    public interface IClippable
    {
        event EventHandler<DependencyArgs> OnPropertyChanged;

        double Width { get; }
        double Height { get; }
        Geometry Clip { get; set; }

    }
}
