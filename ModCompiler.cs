using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WorldBoxModLoader
{
    internal sealed class ModCompiler
    {
        public static List<ModConstants> CompiledMods { get; private set; } = new List<ModConstants>();
        public static string worldBoxDirectory;
        public static string worldBoxDataDirectory;
        private static List<string> defaultReferences = new List<string>();

        public static void Awake()
        {
            worldBoxDataDirectory = Application.dataPath;
            worldBoxDirectory = Path.GetDirectoryName(worldBoxDataDirectory);
            defaultReferences = Directory.GetFiles(worldBoxDataDirectory + @"\Managed\", "*.dll").ToList();
            defaultReferences.Add(Path.GetFullPath(worldBoxDataDirectory + "\\StreamingAssets\\mods\\WorldBoxModLoader.dll"));
            var modsDir = worldBoxDirectory + "\\Mods";
            var compilationsDir = worldBoxDirectory + "\\ModCompilations";

            if (!Directory.Exists(modsDir))
                Directory.CreateDirectory(modsDir);
            if (!Directory.Exists(compilationsDir))
                Directory.CreateDirectory(compilationsDir);

            string[] modDirectories = Directory.GetDirectories(modsDir);
            if (modDirectories.Length == 0)
                return;

            foreach (string modDirectory in modDirectories)
            {
                string newModDirectory = modDirectory + @"\";
                ///may be realisation withput ModConstants.Scripts
                ///Debug.Log(Directory.GetFiles(newModDirectory, "*.cs").Length);
                string outputPath = CompileMod(newModDirectory);
                if (!string.IsNullOrEmpty(outputPath))
                {
                    UtilsInternal.Provide(newModDirectory, out ModConstants modConstants);
                    modConstants.MetaLocation = newModDirectory;
                    modConstants.MetaPath = outputPath;
                    UtilsInternal.UpdateModJson(newModDirectory, modConstants);
                    CompiledMods.Add(modConstants);
                }
                else
                    continue;
            }
            ModLoader.LoadMods(CompiledMods);
        }
        private static string CompileMod(string modDirectory)
        {
            List<MetadataReference> metadataReferences = new List<MetadataReference>();
            List<SyntaxTree> syntaxTreeList = new List<SyntaxTree>();
            List<string> sources = new List<string>();

            UtilsInternal.Provide(modDirectory, out ModConstants modConstants);
            var scripts = modConstants.Scripts;
            var author = modConstants.Author;
            var modName = modConstants.ModName;
            var version = modConstants.Version;
            var enabled = modConstants.Enabled;

            foreach (string script in scripts)
            {
                string fullPath = Path.GetFullPath(modDirectory + script.Replace("/", @"\"));
                if (File.Exists(fullPath))
                {
                    string file = File.ReadAllText(fullPath);
                    sources.Add(file);
                }
            }
            foreach (string path in defaultReferences.Distinct())
            {
                try
                {
                    PortableExecutableReference fromFile = MetadataReference.CreateFromFile(path);
                    metadataReferences.Add(fromFile);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Assembly referencing error: " + ex.Message);
                }
            }
            string depsDir = modDirectory + "dependencies";
            if (Directory.Exists(depsDir))
            {
                string[] files = Directory.GetFiles(depsDir, "*.dll");
                foreach (string file in files)
                {
                    Debug.Log(file);
                    try
                    {
                        PortableExecutableReference fromFile = MetadataReference.CreateFromFile(file);
                        metadataReferences.Add(fromFile);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Assembly referencing error: " + ex.Message);
                    }
                }
            }
            foreach (string source in sources.Distinct())
            {
                try
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
                    syntaxTreeList.Add(syntaxTree);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Parsing error: " + ex.Message);
                }
            }
            if (syntaxTreeList.Count == 0)
            {
                Debug.LogError("No source files could be succesfully parsed");
            }
            else
            {
                CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName: Guid.NewGuid().ToString().Normalize(),
                    syntaxTrees: syntaxTreeList,
                    references: metadataReferences,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                StringBuilder sb = new StringBuilder();
                if (sb.Length > 0)
                {
                    Debug.LogError(sb.ToString());
                }
                else
                {
                    string outputFileName = $"{author}-{modName}-v{version}".Replace(" ", "");
                    string outputPath = Path.GetFullPath(Path.Combine(@"ModCompilations", $"{outputFileName}.dll"));
                    EmitResult emitResult = CSharpFileSystemExtensions.Emit(compilation, outputPath);
                    if (!emitResult.Success)
                        DisplayErrors(emitResult);
                    else
                        return outputPath;
                }
            }
            return null;
        }
        static void DisplayErrors(EmitResult result)
        {
            IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
            diagnostic.IsWarningAsError ||
            diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (Diagnostic diagnostic in failures)
            {
                object[] arguments = new object[] { diagnostic.Id, diagnostic.GetMessage() };
                Debug.LogFormat("\t{0}: {1}", arguments);
            }
        }
    }
}