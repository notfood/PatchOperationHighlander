using System.Xml;

namespace PatchOperation
{
    public class LogText : Verse.PatchOperation
    {
        string value = string.Empty;

        Highlander.LogLevel level = Highlander.LogLevel.Warning;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            Highlander.Log(value, level);
            return true;
        }

        public override void Complete(string modIdentifier)
        {
            // Always succeeds
        }
    }
}