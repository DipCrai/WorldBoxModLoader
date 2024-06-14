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
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Video;

namespace WorldBoxModLoader
{
    public class ModCompiler
    {
        public static List<ModConstants> LoadedMods = new List<ModConstants>();
        private static string[] defaultReferences = new string[] { };

        public static void Awake()
        {
            defaultReferences = Directory.GetFiles(@"worldbox_Data\Managed\", "*.dll");
            if (!Directory.Exists("Mods"))
                Directory.CreateDirectory("Mods");
            if (!Directory.Exists("ModCompilations"))
                Directory.CreateDirectory("ModCompilations");
            string[] modDirectories = Directory.GetDirectories(Path.GetFullPath("Mods"));
            if (modDirectories.Length == 0)
                return;
            foreach (string modDirectory in modDirectories)
            {
                string newModDirectory = modDirectory + @"\";
                ///may be realisation withput ModConstants.Scripts
                ///Debug.Log(Directory.GetFiles(newModDirectory, "*.cs").Length);
                string outputPath = ModCompiler.CompileMod(newModDirectory);
                ModCompiler.Provide(newModDirectory, out ModConstants modConstants);
                ModConstants newConstants = new ModConstants
                {
                    ModName = modConstants.ModName,
                    Author = modConstants.Author,
                    Description = modConstants.Description,
                    Version = modConstants.Version,
                    Scripts = modConstants.Scripts,
                    EntryPoint = modConstants.EntryPoint,
                    MetaLocation = newModDirectory,
                    MetaPath = outputPath
                };
                File.WriteAllText(newModDirectory + "mod.json", JsonConvert.SerializeObject(newConstants));
                LoadedMods.Add(newConstants);
            }
            ModManager.Main();
        }
        private static string CompileMod(string modDirectory)
        {
            List<MetadataReference> metadataReferences = new List<MetadataReference>();
            List<SyntaxTree> syntaxTreeList = new List<SyntaxTree>();
            List<string> source = new List<string>();

            ModCompiler.Provide(modDirectory, out ModConstants modConstants);
            var scripts = modConstants.Scripts;
            var entryPoint = modConstants.EntryPoint;
            var author = modConstants.Author;
            var modName = modConstants.ModName;
            var version = modConstants.Version;

            foreach (string script in scripts)
            {
                string fullPath = Path.GetFullPath(modDirectory + script);
                if (File.Exists(fullPath))
                {
                    string file = File.ReadAllText(fullPath);
                    source.Add(file);
                }
            }
            foreach (string path in defaultReferences.Distinct<string>())
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
            foreach (string text1 in source.Distinct<string>())
            {
                try
                {
                    SyntaxTree text2 = CSharpSyntaxTree.ParseText(text1);
                    syntaxTreeList.Add(text2);
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
                    string outputFileName = $"{author} {modName} v{version}";
                    string newName = "";
                    foreach (char symbol in outputFileName)
                    {
                        char currentSymbol = symbol;
                        if (symbol == ' ')
                            currentSymbol = '-';
                        newName += currentSymbol;
                    }
                    string outputPath = Path.GetFullPath(Path.Combine(@"ModCompilations", $"{newName}.dll"));
                    EmitResult emitResult = CSharpFileSystemExtensions.Emit(compilation, outputPath);
                    if (!emitResult.Success)
                        DisplayErrors(emitResult);
                    else
                        return outputPath;
                }
            }
            return null;
        }
        static void Provide(string modDirectory, out ModConstants modConstantsObject)
        {
            string jsonFile = File.ReadAllText(modDirectory + "mod.json");
            modConstantsObject = JsonConvert.DeserializeObject<ModConstants>(jsonFile);
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