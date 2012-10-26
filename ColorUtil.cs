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
    public class ColorUtil
    {
        public static int HexToInt(string hexstr)
        {
            hexstr = hexstr.ToUpper();

            int hexint = 0;
            char[] hexarr = hexstr.ToCharArray();

            for (int counter = hexarr.Length - 1; counter >= 0; counter--)
            {
                if ((hexarr[counter] >= '0') && (hexarr[counter] <= '9'))
                {
                    hexint += (hexarr[counter] - 48) * ((int)(Math.Pow(16, hexarr.Length - 1 - counter)));
                }
                else
                {
                    if ((hexarr[counter] >= 'A') && (hexarr[counter] <= 'F'))
                    {
                        hexint += (hexarr[counter] - 55) * ((int)(Math.Pow(16, hexarr.Length - 1 - counter)));
                    }
                    else
                    {
                        hexint = 0;
                        break;
                    }
                }
            }
            return hexint;
        }

        public static string IntToHex(int hexint)
        {
            int counter = 1;
            int reminder;
            string hexstr = string.Empty;
            while (hexint + 15 > Math.Pow(16, counter - 1))
            {
                reminder = (int)(hexint % Math.Pow(16, counter));
                reminder = (int)(reminder / Math.Pow(16, counter - 1));
                if (reminder <= 9)
                {
                    hexstr = hexstr + (char)(reminder + 48);
                }
                else
                {
                    hexstr = hexstr + (char)(reminder + 55);
                }
                hexint -= reminder;
                counter++;
            }
            return Reverse(hexstr);
        }

        public static string Reverse(string s)
        {
            String reversed = string.Empty;
            for (int counter = s.Length - 1; counter >= 0; counter--)
            {
                reversed = reversed + s[counter];
            }
            return reversed;
        }

        public static string IntToHex(int hexint, int length)
        {
            string hexstr = IntToHex(hexint);
            string ret = string.Empty;
            if (hexstr.Length < length)
            {
                for (int counter = 0; counter < (length - hexstr.Length); counter++)
                {
                    ret = ret + "0";
                }
            }
            return ret + hexstr;
        }

        public static Color HexToColor(string hexString)
        {
            Color actColor = Colors.Black;
            int a = 0;
            int r = 0;
            int g = 0;
            int b = 0;

            if (hexString.StartsWith("#"))
            {
                if (hexString.Length == 7)
                {
                    r = HexToInt(hexString.Substring(1, 2));
                    g = HexToInt(hexString.Substring(3, 2));
                    b = HexToInt(hexString.Substring(5, 2));
                    actColor = Color.FromRgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                }
                else if (hexString.Length == 9)
                {
                    a = HexToInt(hexString.Substring(1, 2));
                    r = HexToInt(hexString.Substring(3, 2));
                    g = HexToInt(hexString.Substring(5, 2));
                    b = HexToInt(hexString.Substring(7, 2));
                    actColor = Color.FromArgb(Convert.ToByte(a), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                }
            }

            return actColor;
        }

        public static string ColorToHex(Color actColor)
        {
            return "#" + IntToHex(actColor.A, 2) + IntToHex(actColor.R, 2) + IntToHex(actColor.G, 2) + IntToHex(actColor.B, 2);
        }
    }
}
