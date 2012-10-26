/*using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Diagnostics;

namespace StringTemplate.Controls
{
    public enum VerticalAlignment
    {
        Left,
        Center,
        Right,
        Indent
    }

    public enum HorizontalAlignment
    {
        Top,
        Middle,
        Bottom
    }

    public class LinkableTextBlock : Control
    {
        private const string WEB_PREFIX = @"http:\\";
        private const string EMAIL_PREFIX = @"mailto:";
        private const string NEW_WINDOW = @"_blank";
        private const string HIGHLIGHT = "_highlight";

        private readonly Brush OverColor = new SolidColorBrush(Colors.Blue);
        private const TextDecorations OverDecorations = TextDecorations.Underline;
        private readonly Brush ClickColor = new SolidColorBrush(Colors.LightGray);

        private Color originalColor = Colors.Black;
        private TextDecorations originalDecorations = TextDecorations.None;
        private Nullable<double> originalLeft = null;
        private Nullable<double> originalTop = null;

        private Rectangle highlight;
        private RectangleGeometry clip;
        private string link;
        private bool clicked;

        protected TextBlock block;

        public LinkableTextBlock() : base()
        {
            block = (TextBlock)InitializeFromXaml("<TextBlock/>");
            VerticalAlignment = VerticalAlignment.Left;
            HorizontalAlignment = HorizontalAlignment.Top;
            TextWrapping = TextWrapping.NoWrap;

            Loaded += new EventHandler(HandleLoaded);
        }

        private void HandleLoaded(object sender, EventArgs e)
        {
            if (block.Width > 0) { block.Width = Width; }
            if (block.Height > 0) { block.Height = Height; }

            SetClip();
            Justify();
            InitializeHighlight();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            block.MouseEnter += new MouseEventHandler(OnMouseEnter);
            block.MouseLeave += new EventHandler(OnMouseLeave);
            block.MouseLeftButtonDown += new MouseEventHandler(OnMouseClick);
            block.MouseLeftButtonUp += new MouseEventHandler(OnMouseRelease);
        }

        private void InitializeHighlight()
        {
            highlight = new Rectangle();
            highlight.SetValue<string>(Canvas.NameProperty, Name + HIGHLIGHT);
            highlight.Width = block.ActualWidth;
            highlight.Height = block.ActualHeight;
            highlight.SetValue<double>(Canvas.TopProperty, (double)block.GetValue(Canvas.TopProperty));
            highlight.SetValue<double>(Canvas.LeftProperty, (double)block.GetValue(Canvas.LeftProperty));
            highlight.SetValue<int>(Canvas.ZIndexProperty, (int)block.GetValue(Canvas.ZIndexProperty) - 1);
            highlight.Fill = ClickColor;
            highlight.Visibility = Visibility.Collapsed;
            ((Canvas)Parent).Children.Add(highlight);
        }

        private void Justify()
        {
            if (!originalLeft.HasValue)
            {
                originalLeft = (double)GetValue(Canvas.LeftProperty);
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Left:
                    SetValue<double>(Canvas.LeftProperty, originalLeft.Value);
                    break;
                case VerticalAlignment.Center:
                    SetValue<double>(Canvas.LeftProperty, (Width - ActualWidth) / 2.0 + originalLeft.Value);
                    break;
                case VerticalAlignment.Right:
                    SetValue<double>(Canvas.LeftProperty, Width - ActualWidth + originalLeft.Value);
                    break;
                case VerticalAlignment.Indent:
                    Text = "     " + Text;
                    break;
                default:
                    throw new ArgumentException("Unknown VerticalAlignment.");
            }

            if (!originalTop.HasValue)
            {
                originalTop = (double)GetValue(Canvas.TopProperty);
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Top:
                    SetValue<double>(Canvas.TopProperty, originalTop.Value);
                    break;
                case HorizontalAlignment.Middle:
                    SetValue<double>(Canvas.TopProperty, (Height - ActualHeight) / 2.0 + originalTop.Value);
                    break;
                case HorizontalAlignment.Bottom:
                    SetValue<double>(Canvas.TopProperty, Height - ActualHeight + originalTop.Value);
                    break;
                default:
                    throw new ArgumentException("Unknown HorizontalAlignment");
            }
        }

        public override void SetValue<T>(DependencyProperty property, T obj)
        {
            if (property == TextBlock.TextWrappingProperty)
            {
                TextWrapping = (TextWrapping)Enum.Parse(typeof(TextWrapping), obj.ToString(), true);
            }
            else if (property == TextBlock.WidthProperty)
            {
                Width = Convert.ToDouble(obj);
            }
            else if (property == TextBlock.HeightProperty)
            {
                Height = Convert.ToDouble(obj);
            }

            base.SetValue<T>(property, obj);
        }

        public override object GetValue(DependencyProperty property)
        {
            if (property == TextBlock.ActualHeightProperty)
            {
                return ActualHeight;
            }
            else
            {
                return base.GetValue(property);
            }
        }

        private void SetClip()
        {
            if (clip == null)
            {
                clip = new RectangleGeometry();
            }

            clip.Rect = new Rect(0, 0, Width, Height);
            block.Clip = clip;
        }

        private void OnMouseRelease(object sender, MouseEventArgs e)
        {
            if (IsLinked)
            {
                highlight.Visibility = Visibility.Collapsed;

                if (clicked)
                {
                    Navigate();
                }
            }

            clicked = false;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            clicked = true;

            if (IsLinked)
            {
                highlight.Width = block.ActualWidth;
                highlight.Height = block.ActualHeight;
                highlight.SetValue<double>(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty));
                highlight.SetValue<double>(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty));
                highlight.Visibility = Visibility.Visible;
            }
        }

        private void Navigate()
        {
            HtmlPage.Navigate(Link, NEW_WINDOW);                
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            clicked = false;

            if (IsLinked)
            {
                Foreground = new SolidColorBrush(originalColor);
                TextDecorations = originalDecorations;
                highlight.Visibility = Visibility.Collapsed;
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            clicked = false;

            if (IsLinked)
            {
                if (Foreground is SolidColorBrush)
                {
                    originalColor = ((SolidColorBrush)Foreground).Color;
                }

                originalDecorations = TextDecorations;

                Foreground = OverColor;
                TextDecorations = TextDecorations.Underline;
            }
        }

        public bool IsLinked
        {
            get
            {
                return Link != null && Link != string.Empty;
            }
        }

        public bool IsWebLink
        {
            get
            {
                return IsLinked && Link.StartsWith(WEB_PREFIX);
            }
        }

        public bool IsEmailLink
        {
            get
            {
                return IsLinked && Link.StartsWith(EMAIL_PREFIX);
            }
        }

        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                link = value;

                if (!IsWebLink && !IsEmailLink)
                {
                    link = WEB_PREFIX + link;
                }
            }
        }

        public virtual new double Height
        {
            get { return block.Height; }
            set 
            {                
                block.Height = value;
                SetClip();
            }
        }

        public virtual new double Width
        {
            get { return block.Width; }
            set
            {                
                block.Width = value;
                SetClip();
            }
        }

        public virtual new string Name
        {
            get
            {
                if (block.Name == null || block.Name == string.Empty)
                {
                    Name = Convert.ToString(Guid.NewGuid());
                }
                return block.Name;
            }
            set
            {
                block.SetValue<string>(Canvas.NameProperty, value);
            }
        }

        public double ActualHeight
        {
            get { return block.ActualHeight; }
        }

        public double ActualWidth
        {
            get { return block.ActualWidth; }
        }

        public string FontFamily
        {
            get { return block.FontFamily; }
            set { block.FontFamily = value; }
        }

        public double FontSize
        {
            get { return block.FontSize; }
            set { block.FontSize = value; }
        }

        public FontStretches FontStretch
        {
            get { return block.FontStretch; }
            set { block.FontStretch = value; }
        }

        public FontStyles FontStyle
        {
            get { return block.FontStyle; }
            set { block.FontStyle = value; }
        }

        public FontWeights FontWeight
        {
            get { return block.FontWeight; }
            set { block.FontWeight = value; }
        }

        public Brush Foreground
        {
            get { return block.Foreground; }
            set { block.Foreground = value; }
        }

        public string Text
        {
            get { return block.Text; }
            set { block.Text = value; }
        }

        public TextDecorations TextDecorations
        {
            get { return block.TextDecorations; }
            set { block.TextDecorations = value; }
        }

        public TextWrapping TextWrapping
        {
            get { return block.TextWrapping; }
            set { block.TextWrapping = value; }
        }

        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
    }
}*/