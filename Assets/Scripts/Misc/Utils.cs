using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Color[] ScalePixels(Color[] originalPixels, int originalWidth, int originalHeight, int newWidth, int newHeight)
    {
        Color[] newPixels = new Color[newWidth * newHeight];
        float ratioX = (float)originalWidth / newWidth;
        float ratioY = (float)originalHeight / newHeight;
        float x, y;
        int index;
        for (int i = 0; i < newHeight; i++)
        {
            for (int j = 0; j < newWidth; j++)
            {
                x = Mathf.Floor(j * ratioX);
                y = Mathf.Floor(i * ratioY);
                index = (int)(y * originalWidth + x);
                newPixels[i * newWidth + j] = originalPixels[index];
            }
        }
        return newPixels;
    }

    public static Texture2D ScaleTexture( Texture2D originalTexture, int newWidth, int newHeight)
    {
        Texture2D scaledTexture = new Texture2D(newWidth, newHeight);
        scaledTexture.filterMode = FilterMode.Bilinear;
        scaledTexture.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = originalTexture.GetPixels();
        pixels = ScalePixels(pixels, originalTexture.width, originalTexture.height, newWidth, newHeight);
        scaledTexture.SetPixels(pixels);
        scaledTexture.Apply();
        return scaledTexture;
    }

}
