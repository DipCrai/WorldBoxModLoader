using System;

namespace WorldBoxModLoader
{
    [Serializable]
    public class ModConstants
    {
        public string ModName { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string[] Scripts { get; set; }
        public string EntryPoint { get; set; }
        public string MetaLocation { get; set; }
        public string MetaPath { get; set; }
    }
}