using System;
using UnityEngine;

namespace MultiplayerMod
{
    public class Mod
    {
        public static void Main()
        {
            Debug.Log("Init Multiplayer Mod");
            GameObject windowDrawer = new GameObject("MultiplayerMod.WindowDriwer");
            windowDrawer.AddComponent<WindowDrawer>();
            UnityEngine.Object.DontDestroyOnLoad(windowDrawer);
        }
    }
}