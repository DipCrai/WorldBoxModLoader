using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WorldBoxModLoader
{
    internal static class Paths
    {
        public static readonly string StreamingAssetsPath = Combine(Application.streamingAssetsPath);
        public static readonly string NativeModsPath = Combine(StreamingAssetsPath, "Mods");
        public static readonly string WBMLPath = Combine(NativeModsPath, "WBML");
        public static readonly string WBMLAssembliesPath = Combine(WBMLPath, "Assemblies");
        public static readonly string WBMLResourcesPath = Combine(WBMLPath, "Resources");

        private static string Combine(params string[] paths) => new FileInfo(paths.Aggregate("", Path.Combine)).FullName;
    }
}
