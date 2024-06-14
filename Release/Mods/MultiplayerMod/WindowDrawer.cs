using System;
using UnityEngine;

namespace MultiplayerMod
{
    public class WindowDrawer : MonoBehaviour
    {
        public bool Active { get; private set; }
        private Rect _windowRect = new Rect(0, 0, 500, 500);
        private string _IP = "";

        public void OnGUI()
        {
            if (Active)
            {
                _windowRect = GUILayout.Window(1, _windowRect, new GUI.WindowFunction(RenderMenu), "Multiplayer mod", new GUILayoutOption[0]);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
                Active = !Active;
        }

        public void RenderMenu(int id)
        {
            string IPField = GUILayout.TextField(_IP);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("."))
                _IP = IPField + ".";
            if (GUILayout.Button("0"))
                _IP = IPField + "0";
            if (GUILayout.Button("1"))
                _IP = IPField + "1";
            if (GUILayout.Button("2"))
                _IP = IPField + "2";
            if (GUILayout.Button("3"))
                _IP = IPField + "3";
            if (GUILayout.Button("4"))
                _IP = IPField + "4";
            if (GUILayout.Button("5"))
                _IP = IPField + "5";
            if (GUILayout.Button("6"))
                _IP = IPField + "6";
            if (GUILayout.Button("7"))
                _IP = IPField + "7";
            if (GUILayout.Button("8"))
                _IP = IPField + "8";
            if (GUILayout.Button("9"))
                _IP = IPField + "9";
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Clear"))
                _IP = "";
            if (GUILayout.Button("Send") && IPField != "")
            {
                Debug.LogWarning(IPField);
            }
            GUI.DragWindow();
        }
    }
}
