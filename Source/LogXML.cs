using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace PatchOperation
{
    public class LogXML : PatchOperationPathed
    {
        string value = string.Empty;

        Highlander.LogLevel level = Highlander.LogLevel.Warning;

        Formatting formatting = Formatting.Indented;
        
        int indentation = 2;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool useFirstNodeName = false;
            if (string.IsNullOrEmpty(value)) {
                value = xpath;
            }
            if (string.IsNullOrEmpty(value)) {
                useFirstNodeName = true;
            }

            if (xpath.NullOrEmpty()) {
                xpath = "/";
            }

            StringBuilder sb = new StringBuilder();
            foreach (var current in xml.SelectNodes(xpath).Cast<XmlNode>())
            {
                sb.AppendLine(useFirstNodeName ? current.FirstChild.Name : value);
                sb.AppendLine();
                sb.AppendLine(NodeToString(current));
            }

            Highlander.Log(sb.ToString(), level);

            return true;
        }

        string NodeToString(XmlNode xmlNode)
        {
            using (var sw = new StringWriter())
            {
                using (var xw = new XmlTextWriter(sw))
                {
                    xw.Formatting = formatting;
                    xw.Indentation = indentation;

                    xmlNode.WriteTo(xw);
                }
                return sw.ToString();
            }
        }

        public override void Complete(string modIdentifier)
        {
            // Always succeeds
        }
    }
}