using System;
using System.IO;
using UnityEngine;
using NAudio.Wave;
using System.Linq;
using JetBrains.Annotations;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

namespace WorldBoxModLoader
{
    namespace ModAPI
    {
        public static class Utils
        {
            internal static bool modConstantsIsValid = false;
            internal static ModConstants modConstants;
            private static readonly Transform tab_entry_container =
                CanvasMain.instance.canvas_ui.transform.Find("CanvasBottom/BottomElements/BottomElementsMover/TabsButtons");

            private static readonly Transform tab_container = CanvasMain.instance.canvas_ui.transform.Find(
                "CanvasBottom/BottomElements/BottomElementsMover/CanvasScrollView/Scroll View/Viewport/Content/buttons");

            public static Texture2D LoadTexture(string fullPath, FilterMode filterMode = FilterMode.Point)
            {
                if (!modConstantsIsValid)
                    throw new InvalidOperationException("Invalid use of path-dependent function! LoadSprite must be called in the AfterSpawn, Main, or OnLoad function.");
                byte[] data;
                try
                {
                    data = File.ReadAllBytes(Path.Combine(modConstants.MetaLocation, fullPath));
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(exception);
                    return null;
                }
                Texture2D texture = new Texture2D(0, 0);
                if (!texture.LoadImage(data))
                    throw new InvalidDataException("Cannot load texture");
                texture.filterMode = filterMode;
                texture.wrapMode = TextureWrapMode.Clamp;
                return texture;
            }
            public static Sprite LoadSprite(string fullPath, FilterMode filterMode = FilterMode.Point, int pixelsPerUnit = 20)
            {
                if (!modConstantsIsValid)
                    throw new InvalidOperationException("Invalid use of path-dependent function! LoadSprite must be called in the AfterSpawn, Main, or OnLoad function.");
                Texture2D texture = LoadTexture(fullPath, filterMode);
                return texture == null ? null : Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), 0.5f * Vector2.one, pixelsPerUnit);
            }
            public static AudioClip LoadSound(string fullPath)
            {
                if (!modConstantsIsValid)
                    throw new InvalidOperationException("Invalid use of path-dependent function! LoadSprite must be called in the AfterSpawn, Main, or OnLoad function.");
                string path = Path.Combine(modConstants.MetaLocation, fullPath);

                switch (Path.GetExtension(path))
                {
                    case ".mp3":
                        return DecodeMP3();
                    case ".wav":
                        return DecodeWAV();
                    default:
                        return DecodeOther();
                }
                AudioClip DecodeWAV()
                {
                    using (WaveStream stream = new WaveFileReader(path))
                        return WaveStreamToAudioClip(stream);
                }
                AudioClip DecodeOther()
                {
                    using (WaveStream stream = new AudioFileReader(path))
                        return WaveStreamToAudioClip(stream);
                }
                AudioClip DecodeMP3()
                {
                    using (WaveStream stream = new Mp3FileReader(path))
                        return WaveStreamToAudioClip(stream);
                }
                AudioClip WaveStreamToAudioClip(WaveStream stream)
                {
                    int number = stream.WaveFormat.Channels == 1 ? 1 : 0;
                    StereoToMonoProvider16 toMono16 = number != 0 ? null : new StereoToMonoProvider16(stream);
                    byte[] numArray1 = new byte[(stream).Length];
                    int count = number != 0 ? (stream).Read(numArray1, 0, numArray1.Length) : toMono16.Read(numArray1, 0, numArray1.Length);
                    short[] numArray2 = new short[count / 2];
                    Buffer.BlockCopy(numArray1, 0, numArray2, 0, count);
                    AudioClip audioClip = AudioClip.Create(path, numArray2.Length, 1, stream.WaveFormat.SampleRate, false);
                    audioClip.SetData((numArray2).Select<short, float>((b => b / short.MaxValue)).ToArray<float>(), 0);
                    return audioClip;
                }
            }
            public static PowerButton CreateSimplePowerButton([NotNull] string buttonId, UnityAction buttonAction, Sprite buttonIcon)
            {
                PowerButton prefab = UtilsInternal.FindResource<PowerButton>("worldlaws");
                PowerButton button = Object.Instantiate(prefab);

                button.name = buttonId;
                button.icon.sprite = buttonIcon;
                button.type = PowerButtonType.Library;
                button.gameObject.SetActive(true);

                if (buttonAction != null) button.GetComponent<Button>().onClick.AddListener(buttonAction);

                return button;
            }
            public static PowerButton CreateWindowButton([NotNull] string buttonId, [NotNull] string windowId, Sprite buttonIcon)
            {
                PowerButton prefab = UtilsInternal.FindResource<PowerButton>("worldlaws");
                PowerButton button = Object.Instantiate(prefab);

                button.name = buttonId;
                button.icon.sprite = buttonIcon;
                button.open_window_id = windowId;
                button.type = PowerButtonType.Window;
                button.gameObject.SetActive(true);
               
                return button;
            }
            public static void AddButtonToTab(PowerButton button, PowersTab tab, Vector2 position)
            {
                Transform transform = button.transform;
                transform.SetParent(tab.transform);
                transform.localPosition = position;
                transform.localScale = Vector3.one;
                tab.powerButtons.Add(button);
            }
            public static PowersTab GetTab(string tabName)
            {
                if (string.IsNullOrEmpty(tabName)) return null;
                Transform tabTransform = CanvasMain.instance.canvas_ui.transform.Find(
                    $"CanvasBottom/BottomElements/BottomElementsMover/CanvasScrollView/Scroll View/Viewport/Content/buttons/{tabName}");

                return tabTransform == null ? null : tabTransform.GetComponent<PowersTab>();
            }
            public static ScrollWindow CreateEmptyWindow(string pWindowID, string pWindowTitleKey)
            {
                ScrollWindow window = Object.Instantiate(Resources.Load<ScrollWindow>("windows/empty"),
                    CanvasMain.instance.transformWindows);
                window.screen_id = pWindowID;
                window.name = pWindowID;

                return window;
            }
            public static PowersTab CreateTab(string tabName, string titleKey, Sprite tabIcon)
            {
                GameObject tab_entry = Object.Instantiate(UtilsInternal.FindResource<GameObject>("Button_Other"),
                                                          tab_entry_container);

                tab_entry.name = "Button_" + tabName;
                tab_entry.transform.Find("Icon").GetComponent<Image>().sprite = tabIcon;

                PowersTab tab = Object.Instantiate(
                    UtilsInternal.FindResource<GameObject>("Tab_Other").GetComponent<PowersTab>(),
                    tab_container);

                tab.name = "Tab_" + tabName;

                Button tab_entry_button = tab_entry.GetComponent<Button>();
                tab_entry_button.onClick = new Button.ButtonClickedEvent();
                tab_entry_button.onClick.AddListener(() => tab.showTab(tab_entry_button));

                TipButton tab_entry_tip = tab_entry.GetComponent<TipButton>();
                tab_entry_tip.textOnClick = titleKey;
                // Clear tab content
                for (int i = 7; i < tab.transform.childCount; i++)
                {
                    GameObject.Destroy(tab.transform.GetChild(i).gameObject);
                }

                tab.powerButtons.Clear();

                tab.gameObject.SetActive(false);


                tab.gameObject.SetActive(true);
                return tab;
            }
        }
        public static class PowersTabNames
        {
            public const string Main = "Tab_Main";
            public const string Drawing = "Tab_Drawing";
            public const string Kingdoms = "Tab_Kingdoms";
            public const string Creatures = "Tab_Creatures";
            public const string Nature = "Tab_Nature";
            public const string Bombs = "Tab_Bombs";
            public const string Other = "Tab_Other";
        }
    }
}