using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace StringTemplate
{
    [Scriptable]
    public class ScriptingEventArgs : EventArgs
    {
        private string link;

        public ScriptingEventArgs(string link)
            : base()
        {
            this.link = link;
        }

        [Scriptable]
        public string Link
        {
            get
            {
                return link;
            }
        }
    }
}