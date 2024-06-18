using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WorldBoxModLoader;

namespace WorldBoxModLoader
{
    public class WorldBoxMod : MonoBehaviour
    {
        public static Transform Transform;
        private bool initialized;

        private void Start()
        {
            Transform = transform;
        }

        private void Update()
        {
            if (Config.gameLoaded && Config.experimentalMode && !initialized)
            {
                Initialize();
                base.enabled = false;
            }
        }

        private void Initialize()
        {
            initialized = true;
            ModCompiler modCompiler = new ModCompiler();
            ModManager modManager = new ModManager();

            Debug.LogWarning(Path.GetDirectoryName(Application.dataPath));
            ModCompiler.Awake();
        }
    }
}
