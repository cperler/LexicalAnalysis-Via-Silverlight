using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Browser.Serialization;
using System.Diagnostics;

namespace StringTemplate
{
    public class Template
    {
        private string unformatted = string.Empty;
        private string formatted = string.Empty;
        public Dictionary<string, Dictionary<string, object>> parameters = null;

        public Template(string s)
        {
            unformatted = s;
            parameters = new Dictionary<string,Dictionary<string,object>>();
        }

        private void Parse()
        {
            formatted = unformatted;
            foreach (string key in parameters.Keys)
            {                
                Dictionary<string, object> expanders = parameters[key];
                foreach (KeyValuePair<string, object> expand in expanders)
                {
                    if (key != string.Empty)
                    {
                        if (expand.Key != string.Empty)
                        {
                            formatted = formatted.Replace("{" + key + ":" + expand.Key + "}", expand.Value.ToString());
                        }
                        else
                        {
                            int start = formatted.IndexOf("{" + key + ":(");
                            while(start < formatted.Length && start != -1)
                            {
                                int idx = formatted.IndexOf(")", start);
                                string fnParams = formatted.Substring(start, (idx - start)).Replace("{" + key + ":(", string.Empty);
                                string[] items = fnParams.Split(',');
                                string toInsert = expand.Value.ToString();
                                for (int i = 0; i < items.Length; i++)
                                {
                                    toInsert = toInsert.Replace("{$" + (i+1) + "}", items[i]);
                                }
                                formatted = formatted.Replace(formatted.Substring(start, (idx - start + 2)), toInsert);
                                start = formatted.IndexOf("{" + key + ":(", idx);
                            }
                        }
                    }
                    else
                    {
                        formatted = formatted.Replace("{" + expand.Key + "}", expand.Value.ToString());
                    }
                }
            }
        }

        public static implicit operator string(Template st)
        {
            return st.ToString();
        }

        public override string ToString()
        {
            if (formatted == string.Empty)
            {
                Parse();
            }

            return formatted;
        }
    }
}
