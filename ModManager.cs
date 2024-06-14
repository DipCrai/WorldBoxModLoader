using System;
using System.Reflection;
using UnityEngine;

namespace WorldBoxModLoader
{
    public class ModManager
    {
        public static bool Main()
        {
            try
            {
                foreach (ModConstants loadedMod in ModCompiler.LoadedMods)
                {
                    if (loadedMod != null)
                    {
                        ModAPI.modConstants = loadedMod;
                        ModAPI.modConstantsIsValid = true;
                        MethodInfo method = Assembly.LoadFile(loadedMod.MetaPath).GetType(loadedMod.EntryPoint).GetMethod("Main");
                        if (method == null)
                        {
                            Debug.LogWarning(("No static Main\"" + "\" method found in mod " + loadedMod.Author + "." + loadedMod.ModName));
                            ModAPI.modConstantsIsValid = false;
                            return false;
                        }
                        method.Invoke(null, Array.Empty<object>());
                        ModAPI.modConstantsIsValid = false;
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
    }
}
