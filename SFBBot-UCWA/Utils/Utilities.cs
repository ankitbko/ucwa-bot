using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFBBot_UCWA
{
    public class Utilities
    {
        public static String ReduceUriToProtoAndHost(string longUri)
        {
            string reduceUriToProtoAndHost = String.Empty;
            reduceUriToProtoAndHost = new Uri(longUri).Scheme + "://" + new Uri(longUri).Host;
            return reduceUriToProtoAndHost;
        }

        // This function converts HTML code to plain text
        // Any step is commented to explain it better
        // You can change or remove unnecessary parts to suite your needs
        public static string GetMessageFromHref(string hrefString)
        {
            string message = hrefString.Substring(hrefString.IndexOf(',') + 1);
            message = message.Replace('+', ' ');
            if (message.IndexOf("%0d") > -1)
                return message.Remove(message.IndexOf("%0d"));
            else
                return message;
        }

        public static string GetMessageFromHtml(string hrefString)
        {
            string message = hrefString.Substring(hrefString.LastIndexOf("%3b%22%3e")).Replace("%3b%22%3e", "");// ("data:text/html;charset=utf-8,%3cspan+style%3d%22font-size%3a10pt%3bfont-family%3aSegoe+UI%3bcolor%3a%23000000%3b%22%3e", "");// (hrefString.IndexOf(',') + 1);
            message = message.Replace("%3c%2fspan%3e", "");
            message = message.Replace("+", " ");
            return message;
        }
    }
}
