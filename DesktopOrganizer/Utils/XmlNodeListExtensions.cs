using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DesktopOrganizer.Utils
{
    public static class XmlNodeListExtensions
    {
        public static List<XmlNode> ToList(this XmlNodeList node_list)
        {
            return new List<XmlNode>(node_list.Cast<XmlNode>());
        }
    }
}
