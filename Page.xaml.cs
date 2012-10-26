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
using System.Diagnostics;
using System.Windows.Browser.Serialization;
using System.IO;
using System.Reflection;

namespace StringTemplate
{
    public partial class Page : Canvas
    {
        public void Page_Loaded(object o, EventArgs e)
        {
//            try
  //          {
                InitializeComponent();

                Canvas c = XamlReader.Load(
                        "<Canvas xmlns:app='clr-namespace:StringTemplate;assembly=ClientBin/StringTemplate.dll'><app:LayoutPanel Width='100' Height='100'><app:LinkableTextBlock Text='testing...'/></app:LayoutPanel></Canvas>"
                    ) as Canvas;
                LayoutPanel lp = c.Children[0] as LayoutPanel;
                c.Children.Remove(lp);

                c = XamlReader.Load(@"<Canvas xmlns:app='clr-namespace:StringTemplate;assembly=ClientBin/StringTemplate.dll'><app:LinkableTextBlock Text='1 2 3' VAlign='Right'/></Canvas>") as Canvas;
                LinkableTextBlock lt = c.Children[0] as LinkableTextBlock;
                c.Children.Remove(lt);

                lp.Children.Add(lt);
                Children.Add(lp);
                /*
                Rectangle border = new Rectangle();
                border.Width = 300;
                border.Height = 100;
                border.RadiusX = 10;
                border.RadiusY = 10;
                border.StrokeThickness = 1;
                border.Stroke = new SolidColorBrush(Colors.Black);
                border.SetValue<double>(Canvas.LeftProperty, 100);
                border.SetValue<double>(Canvas.TopProperty, 100);
                border.SetValue<int>(Canvas.ZIndexProperty, 5);
                Children.Add(border);

                StackLayout sl = new StackLayout();
                sl.ExpansionMode = ExpansionMode.Normal;
                sl.Width = 300;
                sl.Height = 100;
                sl.SetValue<double>(Canvas.LeftProperty, 100);
                sl.SetValue<double>(Canvas.TopProperty, 100);
                Children.Add(sl);

                LayoutPanel row1 = new LayoutPanel();
                row1.ExpansionMode = ExpansionMode.Actual;
                row1.Width = sl.Width;
                row1.Height = sl.Height;
                sl.Children.Add(row1);

                LinkableTextBlock atb1 = new LinkableTextBlock();
                atb1.FontSize = 22;
                atb1.FontWeight = FontWeights.Bold;
                atb1.Text = "Node 1";
                row1.Children.Add(atb1);

                LayoutPanel row2 = new LayoutPanel();
                row2.ExpansionMode = ExpansionMode.Actual;
                row2.Width = sl.Width;
                row2.Height = sl.Height;
                sl.Children.Add(row2);

                LinkableTextBlock atb2_left = new LinkableTextBlock();
                atb2_left.FontSize = 10;
                atb2_left.FontFamily = "Arial";
                atb2_left.Text = "admin";
                atb2_left.Link = "mailto:csperler@yahoo.com";
                row2.Children.Add(atb2_left);

                LinkableTextBlock atb2_right = new LinkableTextBlock();
                atb2_right.FontSize = 10;
                atb2_right.FontFamily = "Arial";
                atb2_right.VerticalAlignment = VerticalAlignment.Right;
                atb2_right.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                atb2_right.Link = "mailto:csperler@yahoo.com";
                row2.Children.Add(atb2_right);

                LayoutPanel row3 = new LayoutPanel();
                row3.Width = sl.Width;
                row3.Height = sl.Height;
                sl.Children.Add(row3);

                LinkableTextBlock atb3 = new LinkableTextBlock();
                atb3.FontSize = 10;
                atb3.FontFamily = "Lucida Console";
                atb3.VerticalAlignment = VerticalAlignment.Indent;
                atb3.Text = "this is node #1 this is node #2 this is node #3 this is node #4 this is node #5 this is node #6 this is node #7";
                row3.Children.Add(atb3);

                Rectangle border1 = new Rectangle();
                border1.Height = row1.Height;
                border1.Width = row1.Width;
                border1.SetValue<double>(Canvas.LeftProperty, 0);
                border1.SetValue<double>(Canvas.TopProperty, 0);
                border1.Fill = new SolidColorBrush(ColorUtil.HexToColor("#33FF0000"));
                border1.SetValue<int>(Canvas.ZIndexProperty, -1);
                // row1.Children.Add(border1);

                Rectangle border2 = new Rectangle();
                border2.Height = row2.Height;
                border2.Width = row2.Width;
                border2.SetValue<double>(Canvas.LeftProperty, 0);
                border2.SetValue<double>(Canvas.TopProperty, 0);
                border2.Fill = new SolidColorBrush(ColorUtil.HexToColor("#3333AA22"));
                border2.SetValue<int>(Canvas.ZIndexProperty, -1);
                //row2.Children.Add(border2);

                Rectangle border3 = new Rectangle();
                border3.Width = row3.Height;
                border3.Height = row3.Width;
                border3.SetValue<double>(Canvas.LeftProperty, 0);
                border3.SetValue<double>(Canvas.TopProperty, 0);
                border3.Fill = new SolidColorBrush(ColorUtil.HexToColor("#33AB42FC"));
                border3.SetValue<int>(Canvas.ZIndexProperty, -1);

                //row3.Children.Add(border3);

                //border2.Height = row.Height;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //MouseMove += new MouseEventHandler(Page_MouseMove);
            //MouseLeave += new EventHandler(Page_MouseLeave);
            /*            List<LinkableTextBlock> blocks = new List<LinkableTextBlock>();
                        blocks.Add(atb);
                        blocks.Add(atb2);

                        TextBlockRow mtb = new TextBlockRow();
                        mtb.SetValue<double>(Canvas.TopProperty, 100);
                        mtb.SetValue<double>(Canvas.LeftProperty, 100);
                        mtb.Width = 100;
                        mtb.Height = 1000;
                        mtb.Blocks.Add(atb);
                        mtb.Blocks.Add(atb2);
                        Children.Add(mtb);

                        Test();*/
        }

        void Page_MouseLeave(object sender, EventArgs e)
        {
            ((ScaleTransform)RenderTransform).ScaleX = 1;
            ((ScaleTransform)RenderTransform).ScaleY = 1;
        }

        void Page_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(null);

            ScaleTransform st = new ScaleTransform();
            st.ScaleX = (pt.X - (double)GetValue(Canvas.LeftProperty)) / Width;
            st.ScaleY = (pt.Y - (double)GetValue(Canvas.TopProperty)) / Height;
            RenderTransform = st;
        }

        private void Test()
        {
            string master = "{tb:(subject,subject,subject)}<br><center><a>{tb:(author,author,username)}</a></center>";
            Template st = new Template(master);

            Dictionary<string, object> e = new Dictionary<string, object>();
            e.Add("", "{tbStart} Name='{id}_{$1}' {class:{$2}}{tbClose}{{$3}}{tbEnd}");
            st.parameters.Add("tb", e);

            Dictionary<string, object> c = new Dictionary<string, object>();
            c.Add("subject", "FontSize='20' FontFamily='Tahoma'");
            c.Add("author", "FontSize='11' FontFamily='Verdana'");
            st.parameters.Add("class", c);

            Dictionary<string, object> g = new Dictionary<string, object>();
            g.Add("id", "root");
            g.Add("subject", "this is the root");
            g.Add("username", "mr rooty root");
            g.Add("tbStart", "<TextBlock Canvas.Top='300' Canvas.Left='300' Width='300' Height='100'");
            g.Add("tbClose", ">");
            g.Add("tbEnd", "</TextBlock>");
            st.parameters.Add("", g);

            BlockLoader.Load(st, this, new Point(300, 300));
        }
    }
}