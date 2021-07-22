using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Verse;

namespace PatchOperation
{
    public class Context : Verse.PatchOperation
    {
        string xpath = string.Empty;

        List<Verse.PatchOperation> operations = null;

        [Unsaved]
        string lastFailedNode;
        [Unsaved]
        string lastFailedOperation;
        
        protected override bool ApplyWorker(XmlDocument xml)
        {
            var xmlPart = new XmlDocument();

			foreach (var xmlNode in xml.SelectNodes(xpath).Cast<XmlNode>().ToArray())
			{
                var node =  xmlPart.ImportNode(xmlNode, true);
                xmlPart.AppendChild(node);

                foreach (Verse.PatchOperation operation in operations)
                {
                    if (!operation.Apply(xmlPart))
                    {
                        lastFailedNode = xmlNode.Name;
                        lastFailedOperation = operation.ToString();
                        return false;
                    }
                }

                var parentNode = xmlNode.ParentNode;

                parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node, true), xmlNode);
                parentNode.RemoveChild(xmlNode);
            }
            
			return true;
        }

        public override void Complete(string modIdentifier)
		{
			base.Complete(modIdentifier);
            lastFailedNode = null;
			lastFailedOperation = null;
		}

		public override string ToString()
		{
			int num = ((operations != null) ? operations.Count : 0);
			string text = $"{base.ToString()}(count={num}";
			if (lastFailedOperation != null)
			{
				text = text + ", lastFailedNode=" + lastFailedNode + ", lastFailedOperation=" + lastFailedOperation;
			}
			return text + ")";
		}
    }
}