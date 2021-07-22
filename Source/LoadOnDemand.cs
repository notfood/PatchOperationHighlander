using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using Verse;

namespace PatchOperation
{
    public class LoadOnDemand : Verse.PatchOperation
    {
        static readonly List<string> loaded = new List<string>();

        // Directory where to find the new Defs relative to Mod
        string source = "Defs_OnDemand";

        // If any of these mods is available or the list is empty, proceed
        List<string> mods = null;

        // Load all the folders listed here
        List<string> folders = null;

        // Verbose
        bool verbose = false;

        // It'll only run once
        [Unsaved]
        bool tested;

        // If folders is empty read the list of available folders
        // under source folder and assume they're named by packageId.
        [Unsaved]
        bool autoload;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (tested) return false;

            tested = true;

            var owner = LoadedModManager.RunningMods.First(mcp => mcp.Patches.Any(p => p.sourceFile == this.sourceFile));

            if (verbose) {
                Log.Message($"{owner.Name} :: LoadOnDemand :: Verbosity is on");
            }

            if (folders.NullOrEmpty()) {
                autoload = true;
                folders = ModsConfig.ActiveModsInLoadOrder.Select(m => m.PackageId).ToList();
            }

            if (!mods.NullOrEmpty() && !Highlander.IsAnyActiveByID(mods)) {
                if (verbose) {
                    Log.Message($"{owner.Name} :: LoadOnDemand :: No active mods to load");
                }

                return false;
            }

            bool errors = false;
            foreach (var folder in folders) {
                errors &= LoadDefsInto(xml, owner, folder);
            }

            if (verbose) {
                Log.Message($"{owner.Name} :: LoadOnDemand :: Loaded{(errors?" with errors":"")}");
            }

            return !errors;
        }

        public override void Complete(string modIdentifier)
        {
            // Always succeeds
        }

        // Adapted from LoadedModManager.LoadAllActiveMods
        bool LoadDefsInto(XmlDocument xml, ModContentPack owner, string folder)
        {
            string path = Path.Combine(Path.Combine(owner.RootDir, source), folder);
            if (loaded.Contains(path)) {
                // skipping already loaded paths
                if (verbose) {
                    Log.Message($"{owner.Name} :: LoadOnDemand :: Already loaded {source}/{folder}");
                }

                return false;
            }
            if (!Directory.Exists(path)) {
                // if we aren't autoloading, an explicit folder is missing
                if (!autoload) {
                    Log.Warning($"{owner.Name} :: LoadOnDemand :: Trying to load non-existant folder {source}/{folder}");
                }

                return false;
            }

            var xmlAssetList = DirectXmlLoader.XmlAssetsInModFolder(owner, Path.Combine(source, folder)).ToList();

            var xmlDoc = LoadedModManager.CombineIntoUnifiedXML(xmlAssetList, new Dictionary<XmlNode, LoadableXmlAsset>());
            foreach (XmlNode child in xmlDoc.DocumentElement.ChildNodes) {
                xml.DocumentElement.AppendChild(xml.ImportNode(child, true));
            }

            loaded.Add(path);

            Log.Message($"{owner.Name} :: LoadOnDemand :: Loaded {folder}");

            return true;
        }
    }
}
