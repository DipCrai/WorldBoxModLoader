﻿using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using WorldBoxModLoader.ModAPI;
using Path = System.IO.Path;

namespace WorldBoxModLoader
{
    internal sealed class WorldBoxMod : MonoBehaviour
    {
        private static Assembly ModAssembly = Assembly.GetExecutingAssembly();
        private Vector2 newPosition = new Vector2 (154, 18);
        private bool initialized;

        private void Update()
        {

            if (Config.gameLoaded && Config.experimentalMode && !initialized)
            {
                Initialize();
                enabled = false;
            }
        }

        private void Initialize()
        {
            InitFileSystem();
            initialized = true;
            ModCompiler.Awake();
            SetupModsTab();
        }

        private void SetupModsTab()
        {
            PowersTab tab = Utils.CreateTab("Mods", "Mods", UtilsInternal.GetIcon());

            PowerButton disableAllMods = Utils.CreateSimplePowerButton("Disable all mods", ModLoader.DisableAllMods, 
                UtilsInternal.LoadManifestSprite("DisableAll.png"), "Disables all mods");
            PowerButton enableAllMods = Utils.CreateSimplePowerButton("Enable all mods", ModLoader.EnableAllMods, 
                UtilsInternal.LoadManifestSprite("EnableAll.png"), "Enables all mods");

            Utils.AddButtonToTab(disableAllMods, tab, new Vector2(118, 18));
            Utils.AddButtonToTab(enableAllMods, tab, new Vector2(118, -18));

            foreach (ModConstants mod in ModCompiler.CompiledMods)
            {
                PowerButton modButton = UtilsInternal.CreateModPowerButton(mod.ModName, ModLoader.ToggleMod, 
                    UtilsInternal.LoadSprite(Path.Combine(mod.MetaLocation, "icon.png")), mod, mod.Description);
                Utils.AddButtonToTab(modButton, tab, newPosition);
                if (ModCompiler.CompiledMods.IndexOf(mod) % 2 == 0)
                    newPosition = new Vector2(newPosition.x, -18);
                else
                    newPosition = new Vector2(newPosition.x + 36, 18);
            }
        }

        private void InitFileSystem()
        {
            if (!Directory.Exists(Paths.WBMLAssembliesPath))
            {
                Directory.CreateDirectory(Paths.WBMLAssembliesPath);
            }

            ExtractAssemblies();
            LoadAssemblies();

            void ExtractAssemblies()
            {
                var resources = ModAssembly.GetManifestResourceNames();
                foreach (var resource in resources)
                {
                    if (resource.EndsWith(".dll"))
                    {
                        var file_name = resource.Replace("WorldBoxModLoader.resources.assemblies.", "");
                        var file_path = Path.Combine(Paths.WBMLAssembliesPath, file_name).Replace("-renamed", "");

                        using var stream = ModAssembly.GetManifestResourceStream(resource);
                        using var file = new FileStream(file_path, FileMode.Create, FileAccess.Write);
                        stream.CopyTo(file);
                        stream.Dispose();
                        file.Dispose();
                    }
                }
            }
            void LoadAssemblies()
            {
                foreach (var file_full_path in Directory.GetFiles(Paths.WBMLAssembliesPath, "*.dll"))
                {
                    try
                    {
                        Assembly.LoadFrom(file_full_path);
                    }
                    catch (BadImageFormatException)
                    {
                        Debug.LogError($"" +
                                            $"BadImageFormatException: " +
                                            $"The file {file_full_path} is not a valid assembly.");
                    }
                    catch (FileNotFoundException e)
                    {
                        Debug.LogError($"FileNotFoundException: " +
                                            $"The file {file_full_path} is not found.");
                        Debug.LogError(e.Message);
                        Debug.LogError(e.StackTrace);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception: " +
                                            $"Failed to load assembly {file_full_path}.");
                        Debug.LogError(e.Message);
                        Debug.LogError(e.StackTrace);
                    }
                }
            }
        }
    }
}
