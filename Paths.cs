﻿using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WorldBoxModLoader
{
    internal static class Paths
    {
        public static readonly string WBMLModPath = Assembly.GetExecutingAssembly().Location;
        public static readonly string StreamingAssetsPath = Combine(Application.streamingAssetsPath);
        public static readonly string WorldBoxDataPath = Application.dataPath;
        public static readonly string WorldBoxManagedPath = Combine(WorldBoxDataPath, "Managed");
        public static readonly string WorldBoxPath = Path.GetDirectoryName(WorldBoxDataPath);
        public static readonly string NativeModsPath = Combine(StreamingAssetsPath, "Mods");
        public static readonly string WBMLPath = Combine(NativeModsPath, "WBML");
        public static readonly string WBMLAssembliesPath = Combine(WBMLPath, "Assemblies");
        public static readonly string WBMLResourcesPath = Combine(WBMLPath, "Resources");
        public static readonly string PublicizedAssemblyPath = Combine(WBMLAssembliesPath, "Assembly-CSharp-Publicized.dll");
        public static readonly string ModsPath = Combine(WorldBoxPath, "Mods");
        public static readonly string CompilationsPath = Combine(WorldBoxPath, "ModCompilations");
        public static readonly string SteamappsCommonPath = Path.GetDirectoryName(WorldBoxPath);
        public static readonly string SteamappsPath = Path.GetDirectoryName(SteamappsCommonPath);
        public static readonly string SteamappsWorkshopPath = Combine(SteamappsPath, "workshop");
        public static readonly string SteamappsWorkshopContentPath = Combine(SteamappsWorkshopPath, "content");
        public static readonly string SubscribedModsPath = Combine(SteamappsWorkshopContentPath, "1206560");

        private static string Combine(params string[] paths) => new FileInfo(paths.Aggregate("", Path.Combine)).FullName;
    }
}
