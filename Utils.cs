using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WorldBoxModLoader
{
    internal static class UtilsInternal
    {
        private static Sprite modIcon;
        private static Sprite enableAll;
        private static Sprite disableAll;

        public static void Provide(string modDirectory, out ModConstants modConstantsObject)
        {
            string jsonFile = File.ReadAllText(modDirectory + "mod.json");
            modConstantsObject = JsonConvert.DeserializeObject<ModConstants>(jsonFile);
        }
        public static void UpdateModJson(string modDirectory, ModConstants modConstants)
        {
            File.WriteAllText(modDirectory + "mod.json", JsonConvert.SerializeObject(modConstants, Formatting.Indented));
        }
        public static T FindResource<T>(string name) where T : Object
        {
            T[] first_search = Resources.FindObjectsOfTypeAll<T>();
            T result = null;
            foreach (var obj in first_search)
            {
                if (obj.name.ToLower() == name.ToLower())
                {
                    result = obj;
                }
            }
            return result;
        }
        private static byte[] LoadManifestBytes(string path_under_resources)
        {
            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream($"WorldBoxModLoader.resources.{path_under_resources}");
            byte[] buffer = new byte[s.Length];
            s.Read(buffer, 0, buffer.Length);

            return buffer;
        }
        public static Texture2D LoadManifestTexture(string path_under_resources)
        {
            var s = Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream($"WorldBoxModLoader.resources.{path_under_resources}");
            byte[] buffer = new byte[s.Length];
            s.Read(buffer, 0, buffer.Length);

            Texture2D texture = new Texture2D(0, 0);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(buffer);
            return texture;
        }

        public static Sprite LoadManifestSprite(string path_under_resources, int pixelsPerUnit = 1)
        {
            Texture2D texture = LoadManifestTexture(path_under_resources);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), 0.5f * Vector2.one, pixelsPerUnit);
            return sprite;
        }

        public static Sprite GetIcon()
        {
            if (modIcon != null)
                return modIcon;
            SpriteTextureLoader.addSprite("ui/icons/worldboxmodloader", LoadManifestBytes("Icon.png"));
            var icon = SpriteTextureLoader.getSprite("ui/icons/worldboxmodloader");
            modIcon = icon;
            return icon;
        }
        public static PowerButton CreateModPowerButton([NotNull] string buttonId, UnityAction<ModConstants> buttonAction, Sprite buttonIcon, ModConstants modConstants)
        {
            PowerButton prefab = UtilsInternal.FindResource<PowerButton>("worldlaws");
            PowerButton button = Object.Instantiate(prefab);
            ModPowerButton mbutton = button.gameObject.AddComponent<ModPowerButton>();
            mbutton.ModConstants = modConstants;

            button.name = buttonId;
            button.icon.sprite = buttonIcon;
            button.type = PowerButtonType.Library;
            button.gameObject.SetActive(true);

            if (buttonAction != null) mbutton.onClick += buttonAction;

            return button;
        }
        public static Sprite LoadSprite(string fullPath, FilterMode filterMode = FilterMode.Point, int pixelsPerUnit = 20)
        {
            Texture2D texture = LoadTexture(fullPath, filterMode);
            return texture == null ? null : Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), 0.5f * Vector2.one, pixelsPerUnit);
        }
        public static Texture2D LoadTexture(string fullPath, FilterMode filterMode = FilterMode.Point)
        {
            byte[] data;
            try
            {
                data = File.ReadAllBytes(Path.Combine(fullPath));
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
    }
}
