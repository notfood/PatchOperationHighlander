using System.Collections.Generic;
using System.Linq;
using Verse;
using System.Xml;

namespace PatchOperation
{
    public class Copy : PatchOperationPathed
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            IEnumerable<XmlNode> destinations = xml.SelectNodes(toXpath).Cast<XmlNode>();
            if (!destinations.EnumerableNullOrEmpty())
            {
                foreach (XmlNode xmlNode in xml.SelectNodes(xpath).Cast<XmlNode>().ToArray())
                {
                    foreach (XmlNode xmlrecipient in destinations)
                    {
                        xmlrecipient.InnerXml = xmlNode.InnerXml;
                        if (append != null)
                        {
                            xmlrecipient.InnerText = xmlrecipient.InnerText + append;
                        }
                        result = true;
                    }
                }
            }
            return result;
        }
        protected string toXpath;
        protected string append = null;
    }
}
