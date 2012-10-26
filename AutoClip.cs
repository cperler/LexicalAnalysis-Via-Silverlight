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
    public class AutoClip
    {
        private List<UIElement> children;
        private RectangleGeometry geometry;
        private IClippable parent;

        public AutoClip(IClippable parent)
        {
            this.parent = parent;

            children = new List<UIElement>();
            geometry = new RectangleGeometry();
            geometry.Rect = new Rect(0, 0, parent.Width, parent.Height);
            parent.Clip = geometry;
            parent.OnPropertyChanged += new EventHandler<DependencyArgs>(OnPropertyChanged);
        }

        private void OnPropertyChanged(object sender, DependencyArgs e)
        {            
            if (e.Property == Canvas.WidthProperty)
            {
                geometry = new RectangleGeometry();
                geometry.Rect = new Rect(0, 0, Convert.ToDouble(e.NewValue), parent.Height);
                parent.Clip = geometry;
            }
            else if (e.Property == Canvas.HeightProperty)
            {
                geometry = new RectangleGeometry();
                geometry.Rect = new Rect(0, 0, parent.Width, Convert.ToDouble(e.NewValue));
                parent.Clip = geometry;
            }
            else
            {
                return;
            }

            foreach (UIElement element in children)
            {
                Clip(element);
            }
        }

        public void Clip(UIElement element)
        {
            if (!children.Contains(element))
            {
                children.Add(element);
            }

            RectangleGeometry clone = new RectangleGeometry();
            clone.Rect = new Rect(geometry.Rect.Location, geometry.Rect.Size);
            element.Clip = clone;
        }
    }
}