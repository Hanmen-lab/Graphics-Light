﻿using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Graphics.Textures
{
    internal class ExternalTextureManager : TextureManager
    {
        internal override string SearchPattern { get; set; }

        internal override IEnumerator LoadPreview(string path)
        {
            if (!File.Exists(path)) yield break;

            byte[] textureByte = File.ReadAllBytes(path);
            yield return textureByte;
            if (null == textureByte)
            {
                _assetsToLoad--;
                yield break;
            }
            Texture2D texture = KKAPI.Utilities.TextureUtils.LoadTexture(textureByte);
            yield return texture;
            Texture2D preview = new Texture2D(texture.width, texture.height, texture.format, false);
            UnityEngine.Graphics.CopyTexture(texture, preview);
            Util.ResizeTexture(preview, 64, 64, false);
            Previews.Add(preview);
            TexturePaths.Add(path);
            texture = null;
            _assetsToLoad--;
        }
        internal override IEnumerator LoadTexture(string path, Action<Texture> onChanged = null)
        {
            if (!File.Exists(path)) yield break;

            byte[] textureByte = File.ReadAllBytes(path);
            yield return textureByte;
            if (null == textureByte)
            {
                yield break;
            }
            CurrentTexture = KKAPI.Utilities.TextureUtils.LoadTexture(textureByte);
            yield return CurrentTexture;
            onChanged(CurrentTexture);
        }

        internal void LoadTexture(int index, Action<Texture> onChanged = null)
        {
            if (index < 0 || index >= TexturePaths?.Count)
            {
                return;
            }
            StartCoroutine(LoadTexture(CurrentTexturePath = TexturePaths?[index], onChanged));
        }
    }
}
