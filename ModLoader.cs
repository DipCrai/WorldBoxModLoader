using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WorldBoxModLoader.ModAPI;

namespace WorldBoxModLoader
{
    internal sealed class ModLoader
    {
        private static List<ModConstants> disabledMods = new List<ModConstants>();
        private static List<ModConstants> loadedMods = new List<ModConstants>();
        public static bool LoadMods(List<ModConstants> modConstants)
        {
            try
            {
                foreach (ModConstants loadedMod in modConstants)
                {
                    if (loadedMod != null && !loadedMods.Contains(loadedMod))
                    {
                        if (loadedMod.Enabled)
                        {
                            Utils.modConstants = loadedMod;
                            Utils.modConstantsIsValid = true;
                            MethodInfo method = Assembly.LoadFile(loadedMod.MetaPath).GetType(loadedMod.EntryPoint).GetMethod("Main");
                            if (method == null)
                            {
                                Debug.LogWarning(("No static Main\"" + "\" method found in mod " + loadedMod.Author + "." + loadedMod.ModName));
                                Utils.modConstantsIsValid = false;
                                continue;
                            }
                            method.Invoke(null, Array.Empty<object>());
                            Utils.modConstantsIsValid = false;
                            loadedMods.Add(loadedMod);
                        }
                        else
                            disabledMods.Add(loadedMod);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            return false;
        }

        public static void ToggleMod(ModConstants mod)
        {
            if (mod.Enabled)
            { 
                mod.Enabled = false;
                WorldTip.instance.showToolbarText(mod.ModName + " disabled (relaunch the game)");
            }
            else
            {
                mod.Enabled = true;
                disabledMods.Remove(mod);
                LoadMods(new List<ModConstants>() { mod });
                WorldTip.instance.showToolbarText(mod.ModName + " enabled");
            }
            UtilsInternal.UpdateModJson(mod.MetaLocation, mod);
        }

        public static void DisableAllMods()
        {
            WorldTip.instance.showToolbarText("Disabled all mods (relaunch the game)");
            foreach (ModConstants mod in ModCompiler.CompiledMods)
            {
                mod.Enabled = false;
                UtilsInternal.UpdateModJson(mod.MetaLocation, mod);
            }
        }
        public static void EnableAllMods()
        {
            WorldTip.instance.showToolbarText("Enabled all mods");
            foreach (ModConstants mod in ModCompiler.CompiledMods)
            {
                mod.Enabled = true;
                UtilsInternal.UpdateModJson(mod.MetaLocation, mod);
            }
            LoadMods(disabledMods);
        }
    }
}
