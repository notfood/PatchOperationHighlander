using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PatchOperation
{
    public class PatchOperationFindAllMods : Verse.PatchOperation
    {
        private List<string> mods = null;
        private Verse.PatchOperation match = null;
        private Verse.PatchOperation nomatch = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool foundAllMods = mods?.All(mod => ModLister.HasActiveModWithName(mod)) ?? false;
            if (foundAllMods)
            {
                return this.match?.Apply(xml) ?? true;
            }
            else
            {
                return this.nomatch?.Apply(xml) ?? true;
            }
        }
    }
}