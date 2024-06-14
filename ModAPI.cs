using System;
using System.IO;
using UnityEngine;
using NAudio.Wave;
using System.Linq;

namespace WorldBoxModLoader
{
    public class ModAPI
    {
        internal static bool modConstantsIsValid = false;
        public static ModConstants modConstants;
        public static Texture2D LoadTexture(string fullPath, FilterMode filterMode = FilterMode.Point)
        {
            if (!ModAPI.modConstantsIsValid)
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
            if (!ModAPI.modConstantsIsValid)
                throw new InvalidOperationException("Invalid use of path-dependent function! LoadSprite must be called in the AfterSpawn, Main, or OnLoad function.");
            Texture2D texture = ModAPI.LoadTexture(fullPath, filterMode);
            return texture == null ? null : Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), 0.5f * Vector2.one, pixelsPerUnit);
        }
        public static AudioClip LoadSound(string fullPath)
        {
            if (!ModAPI.modConstantsIsValid)
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
    }
}