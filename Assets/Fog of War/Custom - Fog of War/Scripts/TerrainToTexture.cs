using UnityEngine;
using System.Collections;

public class TerrainToTexture : MonoBehaviour
{
    public Terrain Terrain;

    /// <summary>
    /// Visibility texture.
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// Texture pixels.
    /// </summary>
    Color32[] pixels;


    void Start()
    {
        Terrain = GetComponent<Terrain>();

        CreateTexture();
        CreatePixels();
        PaintPixels(Color.yellow);
        PixelsToTexture();
    }

    void CreateTexture()
    {
        // Get terrain size rounded up.
        int terrainWidth = Mathf.CeilToInt(Terrain.terrainData.size.x);
        int terrainHeight = Mathf.CeilToInt(Terrain.terrainData.size.z);

        // Get texture size using power of 2.
        int textureWidth = (int)Mathf.Pow(2, Mathf.CeilToInt(Mathf.Log(terrainWidth, 2)));
        int textureHeight = (int)Mathf.Pow(2, Mathf.CeilToInt(Mathf.Log(terrainHeight, 2)));

        // Create texture.
        Texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, true, true);
    }

    private void CreatePixels()
    {
        pixels = new Color32[Texture.width * Texture.height];
    }

    private void PaintPixels(Color color)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
    }

    private void PixelsToTexture()
    {
        Texture.SetPixels32(pixels);
    }

}
