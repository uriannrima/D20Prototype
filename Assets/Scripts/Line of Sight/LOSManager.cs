using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Size parameters.
/// </summary>
[Serializable]
public class SizeParameters
{
    /// <summary>
    /// Terrain used as reference for FoW.
    /// </summary>
    public Terrain Terrain;

    /// <summary>
    /// FoW Width.
    /// </summary>
    public int Width = -1;

    /// <summary>
    /// FoW Height.
    /// </summary>
    public int Height = -1;

    /// <summary>
    /// FoW Quality.
    /// </summary>
    public float Quality = 1;

    /// <summary>
    /// Should use High Detail Texture for FoW.
    /// </summary>
    public bool HighDetailTexture = false;
}


/// <summary>
/// Visual parameter
/// </summary>
[Serializable]
public class VisualParameters
{
    /// <summary>
    /// Itensity of AoI.
    /// </summary>
    [Range(0, 255)]
    public int AOIntensity = 128;

    /// <summary>
    /// Interpolation rate.
    /// </summary>
    [Range(0, 1024)]
    public int InterpolationRate = 512;

    /// <summary>
    /// ?
    /// </summary>
    public float GrayscaleDecayDuration = 300;

    /// <summary>
    /// ?
    /// </summary>
    public bool RevealOnEntityDiscover = true;
}

/// <summary>
/// Enable Height Blockers.
/// </summary>
[Serializable]
public class HeightBlockerParameters
{
    public bool Enable = true;
    public bool AllowOwnTeamBlockers = false;
}

/// <summary>
/// Pixel Color Mapping RGB:
/// Red: Visibility status
/// Green: Fog status
/// Blue: Grayscale status
/// 
/// If Visible: 255,255,0
/// If Fogged: 0,255,0
/// If Undiscovered: 0,0,0
/// </summary>
public static class ColorMapping
{
    /// <summary>
    /// Fogged Color (0, 255, 0, 255).
    /// </summary>
    public static readonly Color32 Fogged = new Color32(0, 255, 0, 255);

    /// <summary>
    /// Visible Color (255, 255, 0, 255).
    /// </summary>
    public static readonly Color32 Visible = new Color32(255, 255, 0, 255);

    /// <summary>
    /// Undiscovered Color  (0, 0, 0, 255).
    /// </summary>
    public static readonly Color32 Undiscovered = new Color32(0, 0, 0, 255);
}


[ExecuteInEditMode]
public class LOSManager : BaseManager<LOSManager>
{
    /// <summary>
    /// Parameters for Fog of War.
    /// </summary>
    public SizeParameters Size;

    public bool StartRevealed = false;

    /// <summary>
    /// Getters and Setters for Size Parameter.
    /// </summary>
    public Terrain Terrain { get { return Size.Terrain; } }
    public int Width { get { return Size.Width; } }
    public int Height { get { return Size.Height; } }
    public float Scale { get { return Size.Quality; } }
    public bool HighDetailTexture { get { return Size.HighDetailTexture; } }

    /// <summary>
    /// Visual definition of Fog of War.
    /// </summary>
    public VisualParameters Visual;

    /// <summary>
    /// Getters and Setters for Visual Parameter.
    /// </summary>
    public int AOIntensity { get { return Visual.AOIntensity; } }
    public int InterpolationRate { get { return Visual.InterpolationRate; } }
    public float GrayscaleDecayDuration { get { return Visual.GrayscaleDecayDuration; } }
    public bool RevealOnEntityDiscover { get { return Visual.RevealOnEntityDiscover; } }

    /// <summary>
    /// Height properties of Fog of War.
    /// </summary>
    public HeightBlockerParameters HeightBlockers;

    /// <summary>
    /// Getters and Setters for Height Blockers Parameters.
    /// </summary>
    public bool EnableHeightBlockers { get { return HeightBlockers.Enable; } }
    public bool AllowOwnTeamHeightBlockers { get { return HeightBlockers.AllowOwnTeamBlockers; } }

    /// <summary>
    /// FoW Texture
    /// As it only storage visibility, don't need to be 3D.
    /// </summary>
    Texture2D losTexture;

    /// <summary>
    /// Each of FoW Texture pixels.
    /// </summary>
    Color32[] pixels;

    /// <summary>
    /// Pixels lerped.
    /// </summary>
    Color32[] lerpPixels;

    /// <summary>
    /// Height of a certain position X, Y.
    /// </summary>
    float[,] blockHeights;

    /// <summary>
    /// Timer cache used to grayScale.
    /// </summary>
    float timer = 0;

    /// <summary>
    /// Frame cache used to update AO and LoS each pair number.
    /// So, it makes the FoW update one time for each two frames.
    /// </summary>
    int frameId = 0;

    /// <summary>
    /// Clone of HeightBlockers used for calculations.
    /// </summary>
    float[,] terrainHeightsCache;

    // List of entities that interact with LOS.
    [HideInInspector]
    public List<LOSEntity> Entities = new List<LOSEntity>();


    // List of entities currently animating their LOS, like moving.
    [HideInInspector]
    public List<LOSEntity> AnimatingEntities = new List<LOSEntity>();

    /// <summary>
    /// Enable Fog of War on Editor Mode.
    /// </summary>
    public bool PreviewInEditor = true;

    /// <summary>
    /// Used to determine when the user changes a field that requires
    /// the texture to be recreated
    /// </summary>
    private int previewParameterHash = 0;

    /// <summary>
    /// Generate hash for Size parameter.
    /// If something changes on the size parameters, the game should regenerate everything.
    /// </summary>
    /// <returns></returns>
    private int GenerateParameterHash()
    {
        return (Width + Height * 1024) + Scale.GetHashCode() + HighDetailTexture.GetHashCode();
    }

    /// <summary>
    /// Get Size for FoW Texture accordingly to TerrainSize (Width and Height) and Scale (Quality).
    /// </summary>
    /// <param name="desired">The perfect size.</param>
    /// <param name="terrainSize">The terrain size.</param>
    /// <param name="scale">Kinda like quality.</param>
    /// <returns></returns>
    private int SizeFromParams(int desired, float terrainSize, float scale)
    {
        int size = 128;

        // Desired and defined size.
        if (desired > 0)
        {
            size = Mathf.CeilToInt(desired * scale);
        }
        // Using TerrainSize as reference.
        else if (terrainSize > 0)
        {
            size = Mathf.CeilToInt(terrainSize * scale);
        }

        // Don't know why exactly 4 and 512, I guess that works out as resolution, and you need at least 4 squares.
        return Mathf.Clamp(size, 4, 512);
    }

    /// <summary>
    /// Return texture format accordingly to Visual Parameters.
    /// </summary>
    /// <returns></returns>
    private TextureFormat TextureFormatFromVisual()
    {
        if (HighDetailTexture)
        {
            if (AOIntensity > 0)
            {
                return TextureFormat.ARGB32;
            }
            else
            {
                return TextureFormat.RGB24;
            }
        }
        else
        {
            if (AOIntensity > 0)
            {
                return TextureFormat.ARGB4444;
            }
            else
            {
                return TextureFormat.RGB565;
            }
        }
    }

    /// <summary>
    /// Paint each pixel from FoW Texture to black.
    /// </summary>
    private void PaintPixelsBlack()
    {
        for (int p = 0; p < pixels.Length; ++p)
        {
            if (StartRevealed)
            {
                pixels[p] = Color.white;
            }
            else
            {
                pixels[p] = Color.black;
            }
        }
    }

    /// <summary>
    /// Create _FOWTex and _FOWTex_ST global references to Shader.
    /// </summary>
    /// <param name="width">FoW Texture width.</param>
    /// <param name="height">FoW Texture height.</param>
    private void DefineShaderGlobalReferences(int width, int height)
    {
        if (Terrain != null)
        {
            // Set FoW Texture as a global reference for Shaders.
            Shader.SetGlobalTexture("_FOWTex", losTexture);

            // Set Vector4 as global reference for Shaders.
            // Don't know exactly what is done.
            Shader.SetGlobalVector("_FOWTex_ST",
                new Vector4(
                    Scale / width, Scale / height,
                    (0.5f - Scale * 0.5f) / width, (0.5f - Scale * 0.5f) / height
                )
            );
        }
    }

    /// <summary>
    /// Create Fog of War Texture.
    /// </summary>
    void InitializeTexture()
    {
        // Width of the Fog of War Texture
        int width = SizeFromParams(Width, Terrain != null ? Terrain.terrainData.size.x : 0, Scale);

        // Height of the Fog of War Texture
        int height = SizeFromParams(Height, Terrain != null ? Terrain.terrainData.size.z : 0, Scale);

        // Check if Texture already exists, if so, destroy it.
        if (losTexture != null)
        {
            DestroyImmediate(losTexture);
        }

        // Set heights to null
        blockHeights = null;

        // Get TextureFormat.
        TextureFormat texFormat = TextureFormatFromVisual();

        // Create FoW Texture.
        losTexture = new Texture2D(width, height, texFormat, false);

        // Get pixels from texture.
        pixels = losTexture.GetPixels32();

        // Paint each pixel to black.
        PaintPixelsBlack();

        // Put all pixels back to the texture.
        losTexture.SetPixels32(pixels);

        lerpPixels = null;

        // Define references to Shader.
        DefineShaderGlobalReferences(width, height);

        Debug.Log("FOW Texture created, " + width + " x" + height);
    }

    /// <summary>
    /// Start method.
    /// </summary>
    void Start()
    {
        if (Application.isPlaying) InitializeTexture();
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (PreviewInEditor)
            {
                // Make sure we have a valid texture
                if (losTexture == null || previewParameterHash != GenerateParameterHash())
                {
                    InitializeTexture();
                    previewParameterHash = GenerateParameterHash();
                }
            }
            else
            {
                // Or just use a white texture as placeholder
                Shader.SetGlobalTexture("_FOWTex", UnityEditor.EditorGUIUtility.whiteTexture);

                // If there is a FoW Texture in Editor Mode, and it isn't PreviewInEditor, destroy it.
                if (losTexture != null)
                {
                    DestroyImmediate(losTexture);
                }

                losTexture = null;
            }
        }
#endif

        // Normal update, that runs while game is active
        if (losTexture != null)
        {
            // Update any animating entities (update their FOW color)
            for (int e = 0; e < AnimatingEntities.Count; ++e)
            {
                if (AnimatingEntities[e].UpdateFOWColor())
                {
                    AnimatingEntities.RemoveAt(e--);
                }
            }

            // If Update was called, but it isn't running as a game (So, editor mode)
            if (!Application.isPlaying)
            {
                UpdateEditorMode();
            }
            // Not In Editor Mode
            else
            {
                UpdateRunningMode();
            }
        }
    }

    /// <summary>
    /// Update used when the game is running on editor mode.
    /// </summary>
    private void UpdateEditorMode()
    {
        // Refresh the map each frame
        for (int p = 0; p < pixels.Length; ++p)
        {
            // Turn all pixels to fogged.
            pixels[p] = ColorMapping.Fogged;
        }

        // Add LOS and AO for all entities
        foreach (var entity in Entities)
        {
            RevealLOS(entity, entity.IsRevealer ? 255 : 0, 255, 255);
            if (entity.EnableAO && (AOIntensity > 0 || EnableHeightBlockers))
            {
                var bounds = entity.Bounds;
                AddAO(bounds, entity.Height);
            }
        }
    }

    /// <summary>
    /// Update used when the game is running not on editor mode.
    /// </summary>
    private void UpdateRunningMode()
    {
        // Force Full Update is ran only on the first frame of the game
        bool forceFullUpdate = Time.frameCount == 1;

        // Hadle force full update.
        HandleForceFullUpdate(forceFullUpdate);

        // Handle heights blockers.
        HandleHeights(ref forceFullUpdate);

        // Handle grayscale
        HandleGrayScale();

        // Handle Ambient Occlusion and Height Blockers.
        HandleAmbientOcclusionAndHeight(forceFullUpdate);

        // Reveal Line of Sight for units.
        RevealLOSAllEntities(forceFullUpdate);

        // Interpolate pixels.
        HandlePixelsInterpolation();
    }

    /// <summary>
    /// Interpolate changing pixels.
    /// </summary>
    private void HandlePixelsInterpolation()
    {
        bool isChanged = true;
        if (InterpolationRate > 0 && Application.isPlaying)
        {
            if (lerpPixels == null) lerpPixels = pixels.ToArray();
            else
            {
                int rate = Mathf.Max(Mathf.RoundToInt(InterpolationRate * Time.deltaTime), 1);
                for (int p = 0; p < lerpPixels.Length; ++p)
                {
                    byte r = EaseToward(lerpPixels[p].r, pixels[p].r, rate),
                        g = EaseToward(lerpPixels[p].g, pixels[p].g, rate),
                        b = EaseToward(lerpPixels[p].b, pixels[p].b, rate),
                        a = EaseToward(lerpPixels[p].a, pixels[p].a, rate);

                    // If any of the pixels that are being interpolated are equal to the destination pixel, change it.
                    if (isChanged || lerpPixels[p].a != a || lerpPixels[p].r != r || lerpPixels[p].g != g || lerpPixels[p].b != b)
                    {
                        isChanged = true;
                        lerpPixels[p] = new Color32(r, g, b, a);
                    }
                }
            }
        }
        else lerpPixels = null;

        // If some change happened this update
        if (isChanged)
        {
            // Set pixels to the texture
            losTexture.SetPixels32(lerpPixels ?? pixels);

            // Apply it.
            losTexture.Apply();
        }
    }

    /// <summary>
    /// Reveal Line of Sight for All Entities.
    /// </summary>
    private void RevealLOSAllEntities(bool forceFullUpdate)
    {
        // Reveal LOS from all entities
        foreach (var entity in Entities)
        {
            if (entity.IsRevealer) RevealLOS(entity, 255, 255, 330);
        }

        // Entity count.
        int count = 0;

        foreach (var entity in Entities)
        {
            ++count;
            // Entity boundry.
            var rect = entity.Bounds;

            // FoW color at entity broundy.
            var fowColor = GetFOWColor(rect);

            // Get reveal state from that color.
            var visible = GetRevealFromFOW(fowColor);

            // If the actual visibility of the entity don't correspond to the one that was set previously.
            // And entity wasn't hidden and now fogged.
            if (entity.RevealState != visible && !(entity.RevealState == RevealStates.Hidden && visible == RevealStates.Fogged))
            {
                // We set the actual state
                entity.RevealState = visible;

                // If the entity now is fogged and we have reveal on entity discover
                if (visible == RevealStates.Unfogged && RevealOnEntityDiscover)
                {
                    // We make that entity revealed.
                    RevealLOS(rect, 0, entity.Height + entity.transform.position.y, 0, 255, 255);
                }
            }

            // If the actual visibility of this entity isn't hidden, or we are doing an force update.
            if (visible != RevealStates.Hidden || forceFullUpdate)
            {
                // We set its actual FoW color
                entity.SetFOWColor(GetQuantizedFOW(fowColor), !forceFullUpdate);

                // Queue the item for FOW animation
                if (entity.RequiresFOWUpdate && !AnimatingEntities.Contains(entity))
                {
                    AnimatingEntities.Add(entity);
                }
            }
        }
    }

    /// <summary>
    /// Handle Ambient Occlusion and Height Blockers.
    /// </summary>
    private void HandleAmbientOcclusionAndHeight(bool forceFullUpdate)
    {
        ++frameId;
        // Should we update AO
        bool updateAo = (frameId % 2) == 0;

        // If it's time to update AO or it's a full update.
        if (updateAo || forceFullUpdate)
        {
            // Paint all pixels black on the Red color (Make it "invisibile).
            for (int p = 0; p < pixels.Length; ++p)
            {
                pixels[p].r = 0;
                pixels[p].a = 255;
            }

            // If Ambient Occlusion was set or Height blockers was enabled
            if (AOIntensity > 0 || EnableHeightBlockers)
            {
                // Handle Blockers first
                if (Terrain != null && EnableHeightBlockers && blockHeights != null)
                {
                    // If we don't have yeat some cache to terrain height.
                    CreateTerrainHeightCache();

                    // Now that we have the terrainHeight cache, we set it to blockHeights
                    SetBlockHeights();
                }

                HandleEntityHeightBlockers();
            }
        }
    }

    /// <summary>
    /// Add Ambient Occlusion and Height Blockers to be mapped.
    /// </summary>
    private void HandleEntityHeightBlockers()
    {
        // Iterate through all entities.
        foreach (var entity in Entities)
        {
            // Get bound of this entity.
            var bounds = entity.Bounds;

            // If Ambient Occlusion is Enabled and we've set intensity to it.
            if (entity.EnableAO && AOIntensity > 0)
            {
                // Add Ambient Occlusion to this entity.
                AddAO(bounds, entity.Height);
            }

            // If we enabled the height blockers and 
            // Blockers on the same team block its vision, or that entity isn't a revealer.
            if (EnableHeightBlockers && (AllowOwnTeamHeightBlockers || !entity.IsRevealer))
            {
                AddHeightBlocker(bounds, entity.transform.position.y + entity.Height);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private byte EaseToward(byte from, byte to, int amount)
    {
        if (Mathf.Abs(from - to) < amount)
        {
            return to;
        }
        else
        {
            return (byte)(from + (to > from ? amount : -amount));
        }
    }

    /// <summary>
    /// Define Reveal state from parameter color.
    /// </summary>
    /// <param name="px">Color to be checked.</param>
    /// <returns>Reveal state to that color</returns>
    public RevealStates GetRevealFromFOW(Color32 px)
    {
        if (px.r >= 128) return RevealStates.Unfogged;
        if (px.g >= 128) return RevealStates.Fogged;
        return RevealStates.Hidden;
    }

    /// <summary>
    /// Get reveal state from position.
    /// </summary>
    /// <param name="pos">Position to check.</param>
    /// <returns></returns>
    public RevealStates IsVisible(Vector2 pos)
    {
        return GetRevealFromFOW(GetFOWColor(pos));
    }

    /// <summary>
    /// Get reveal state from area.
    /// </summary>
    /// <param name="rect">Area to check.</param>
    /// <returns></returns>
    public RevealStates IsVisible(Rect rect)
    {
        return GetRevealFromFOW(GetFOWColor(rect));
    }

    /// <summary>
    /// Get color from certain position from FoW Texture.
    /// </summary>
    /// <param name="pos">Position of the "pixel".</param>
    /// <returns>RGBA color from that position.</returns>
    public Color32 GetFOWColor(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x * Scale),
            y = Mathf.RoundToInt(pos.x * Scale);
        int p = x + y * losTexture.width;
        return pixels[p];
    }

    /// <summary>
    /// Get maximum or minimum FoW color for parameter color.
    /// </summary>
    /// <param name="px">Color to be quantized.</param>
    /// <returns></returns>
    public Color32 GetQuantizedFOW(Color32 px)
    {
        // If red color is bigger than 128
        // This color means that it its visible (r = 255) and fogged = (g = 255).
        if (px.r >= 128)
        {
            px.r = px.g = px.b = 255;
        }
        // If red color is lesser than 128
        // This color means that it its invisible (r = 0), maybe fogged = (g = 0 || 255).
        else
        {
            px.r = 0;
            px.g = px.g < 128 ? (byte)0 : (byte)255;
        }

        //Return the quantized color.
        return px;
    }

    /// <summary>
    /// Return maximum color from rectangle area.
    /// </summary>
    /// <param name="rect">Rectangle area to get color from.</param>
    /// <returns>Color with the biggest (R, G, B, A).</returns>
    public Color32 GetFOWColor(Rect rect)
    {
        // Valid boundries.
        int xMin, yMin, xMax, yMax;
        GetExtents(rect, 0, out xMin, out yMin, out xMax, out yMax);

        // Color at some point.
        Color32 color = new Color32(0, 0, 0, 0);

        // Get the "bigger" color from rectangle area.
        for (int y = yMin; y <= yMax; ++y)
        {
            for (int x = xMin; x <= xMax; ++x)
            {
                // Index P from FoW Texture.
                int p = x + y * losTexture.width;

                // Save the color.
                color.r = (byte)Mathf.Max(color.r, pixels[p].r);
                color.g = (byte)Mathf.Max(color.g, pixels[p].g);
                color.b = (byte)Mathf.Max(color.b, pixels[p].b);
                color.a = (byte)Mathf.Max(color.a, pixels[p].a);
            }
        }

        return color;
    }

    /// <summary>
    /// Set BlockHeights from TerrainHeightCache.
    /// </summary>
    private void SetBlockHeights()
    {
        for (int y = 0; y < blockHeights.GetLength(0); ++y)
        {
            for (int x = 0; x < blockHeights.GetLength(1); ++x)
            {
                blockHeights[y, x] = terrainHeightsCache[y, x];
            }
        }
    }

    /// <summary>
    /// Create terrain height cache if it doesn't exists.
    /// </summary>
    private void CreateTerrainHeightCache()
    {
        if (terrainHeightsCache == null)
        {
            // Copy block heights.
            terrainHeightsCache = (float[,])blockHeights.Clone();

            // Iterate through heights.
            for (int y = 0; y < blockHeights.GetLength(0); ++y)
            {
                for (int x = 0; x < blockHeights.GetLength(1); ++x)
                {
                    // Get terrain data
                    var terrainData = Terrain.terrainData;

                    // Get specific position from X/Y point.
                    int tx = Mathf.RoundToInt(x * terrainData.heightmapWidth / terrainData.size.x / Scale);
                    int ty = Mathf.RoundToInt(y * terrainData.heightmapHeight / terrainData.size.z / Scale);

                    // Save the height for that specific point.
                    terrainHeightsCache[y, x] = terrainData.GetHeight(tx, ty);
                }
            }
        }
    }

    /// <summary>
    /// Handle first force full update.
    /// </summary>
    /// <param name="forceFullUpdate"></param>
    private void HandleForceFullUpdate(bool forceFullUpdate)
    {
        // Reset all entities to be invisible (as it is the first run).
        if (forceFullUpdate)
        {
            // How many revealers exists.
            int revealerCount = 0;

            // For each entity.
            foreach (var entity in Entities)
            {
                // Change it to Hidden.
                entity.RevealState = RevealStates.Hidden;

                // If it is a reavealer, increment counter.
                if (entity.IsRevealer)
                {
                    revealerCount++;
                }
            }

            // If there is not a single one revealer on this game, tell it.
            if (revealerCount == 0)
            {
                Debug.LogError("No LOSEntity items were marked as revealers! Tick the 'Is Revealed' checkbox for at least 1 item.");
            }
        }
    }

    /// <summary>
    /// Height blocker management.
    /// </summary>
    /// <param name="forceFullUpdate"></param>
    private void HandleHeights(ref bool forceFullUpdate)
    {
        // Ensure we have space to store blocking heights (if enabled)
        if (blockHeights == null && EnableHeightBlockers)
        {
            // Create an array to contain all height information.
            blockHeights = new float[losTexture.height, losTexture.width];

            // We'll need to force update down below.
            forceFullUpdate = true;
        }
    }

    /// <summary>
    /// Handle gray scale from pixels.
    /// </summary>
    private void HandleGrayScale()
    {
        // Decay grayscale from pixels
        if (GrayscaleDecayDuration > 0)
        {
            const int GrayscaleGranularity = 4;

            // Previous grayScale.
            int oldGrayDecay = (int)(256 / GrayscaleGranularity * timer / GrayscaleDecayDuration) * GrayscaleGranularity;

            // Actual timer.
            timer += Time.deltaTime;

            // New grayScale.
            int newGrayDecay = (int)(256 / GrayscaleGranularity * timer / GrayscaleDecayDuration) * GrayscaleGranularity;

            // Differential from new and old.
            int grayDecayCount = newGrayDecay - oldGrayDecay;

            // If its diferente, we must change everyone.
            if (grayDecayCount != 0)
            {
                // Iterate through all pixels and change its grayScale.
                for (int p = 0; p < pixels.Length; ++p)
                {
                    pixels[p].b = (byte)Mathf.Max(pixels[p].b - grayDecayCount, 0);
                }
            }
        }
    }

    /// <summary>
    /// Method used to add an Height blocker.
    /// </summary>
    /// <param name="rect">Rectangle used to define the blocker.</param>
    /// <param name="height">Height of the blocker.</param>
    private void AddHeightBlocker(Rect rect, float height)
    {
        int xMin, yMin, xMax, yMax;
        GetExtents(rect, 0, out xMin, out yMin, out xMax, out yMax);
        for (int y = yMin; y <= yMax; ++y)
        {
            for (int x = xMin; x <= xMax; ++x)
            {
                blockHeights[y, x] = Mathf.Max(blockHeights[y, x], height);
            }
        }
    }

    /// <summary>
    /// Reveal area for Line of Sight entity.
    /// </summary>
    /// <param name="sight">Entity</param>
    /// <param name="los">Line of Sight (0 or 255)</param>
    /// <param name="fow"></param>
    /// <param name="grayscale"></param>
    private void RevealLOS(LOSEntity sight, float los, float fow, float grayscale)
    {
        Rect rect = sight.Bounds;
        RevealLOS(rect, sight.Range, sight.Height + sight.transform.position.y, los, fow, grayscale);
    }

    int[] jCache = new int[1024];
    /// <summary>
    /// Some REAL crazy shit calculation.
    /// </summary>
    /// <param name="rect">Line of Sight Region.</param>
    /// <param name="range">Line of Sight Range.</param>
    /// <param name="height">Line of Sight Height.</param>
    /// <param name="los">Line of Sight Red Factor.</param>
    /// <param name="fow">Line of Sight Green Factor.</param>
    /// <param name="grayscale">Line of Sight Blue Factor.</param>
    private void RevealLOS(Rect rect, float range, float height, float los, float fow, float grayscale)
    {
        // Left, Top, Right and Bottom boundries.
        int xMin, yMin, xMax, yMax;

        // Escalated range
        int rangeI = Mathf.RoundToInt(range * Scale);

        // Left, Top, Right and Bottom boundries without insulation from range.
        int xiMin, yiMin, xiMax, yiMax;

        // Get valid boundries without range insulation
        GetExtents(rect, 0, out xiMin, out yiMin, out xiMax, out yiMax);

        //// Get valid boundries with range insulation
        GetExtents(rect, rangeI, out xMin, out yMin, out xMax, out yMax);

        // If we have Height Blockers enabled and an valid blocHeights.
        if (EnableHeightBlockers && blockHeights != null)
        {
            RevealWithHeight(rect, range, height, los, fow, grayscale, xMin, xMax, yMin, yMax, xiMin, xiMax, yiMin, yiMax);
        }
        else
        {
            RevealWithoutHeight(rect, range, los, fow, grayscale, xMin, xMax, yMin, yMax);
        }
    }

    /// <summary>
    /// Reveal line of sight without using height information.
    /// </summary>
    /// <param name="rect">Line of Sight Region.</param>
    /// <param name="range">Line of Sight Range.</param>
    /// <param name="los">Line of Sight Red Factor.</param>
    /// <param name="fow">Line of Sight Green Factor.</param>
    /// <param name="grayscale">Line of Sight Blue Factor.</param>
    /// <param name="xMin">Left boundry of vision rectangle.</param>
    /// <param name="xMax">Right boundry of vision rectangle.</param>
    /// <param name="yMin">Top boundry of vision rectangle.</param>
    /// <param name="yMax">Bottom boundry of vision rectangle.</param>
    private void RevealWithoutHeight(Rect rect, float range, float los, float fow, float grayscale, int xMin, int xMax, int yMin, int yMax)
    {
        // From yMin to yMax
        for (int y = yMin; y <= yMax; ++y)
        {
            // Get the clamp between yMin and (yMax - 1)
            // About clamp, see here: http://docs.unity3d.com/ScriptReference/Mathf.Clamp.html
            float yIntl = Mathf.Clamp(y, rect.yMin, rect.yMax - 1);

            // From xMin to xMax
            for (int x = xMin; x <= xMax; ++x)
            {
                // We are now going through a "square", from top-left corner to bottom-right corner.
                var nodePos = new Vector2(x, y) / Scale;

                // Get the clmap between xMin and (xMax - 1)
                float xIntl = Mathf.Clamp(x, rect.xMin, rect.xMax - 1);

                // Now we have an internal position, inside this square.
                var intlPos = new Vector2(xIntl, yIntl);
                intlPos = new Vector2(Mathf.Clamp(nodePos.x, rect.xMin, rect.xMax - 1), yIntl);

                // SqrMagnitude returns the Squared Magnitude of the distance for comparrison, wich is much faster
                // The Square Root function takes a lot of time to be calculated, so it's better to have it Squared
                // The only thing that we need to do then, is to Square the comparison distance
                float dist2 = (intlPos - nodePos).sqrMagnitude;
                
                // In this case, as Range is our comparisson distance, and we have de Squared Magnitude
                // We square it, before comparing to the distance.
                float range2 = (range * range);

                // If the distance from this pixel is bigger than the vision range, we continue the for-loop, ignoring below.
                if (dist2 > range2) continue;
                
                // So, now we are inside the radius of the circle.
                const float FadeStart = 2;


                float innerRange = Mathf.Max(range - FadeStart, 0);
                float innerRange2 = innerRange * innerRange;
                float bright = 1;
                if (dist2 > innerRange2)
                {
                    bright = Mathf.Clamp01((range - Mathf.Sqrt(dist2)) / (range - innerRange));
                }
                int p = x + y * losTexture.width;
                pixels[p].r = (byte)Mathf.Max(pixels[p].r, bright * los);
                pixels[p].g = (byte)Mathf.Max(pixels[p].g, bright * fow);
                pixels[p].b = (byte)Mathf.Max(pixels[p].b, Mathf.Clamp(bright * grayscale, 0, 255));
            }
        }
    }

    /// <summary>
    /// Some REAL crazy shit calculation.
    /// </summary>
    /// <param name="rect">Line of Sight Region.</param>
    /// <param name="range">Line of Sight Range.</param>
    /// <param name="height">Line of Sight Height.</param>
    /// <param name="los">Line of Sight Red Factor.</param>
    /// <param name="fow">Line of Sight Green Factor.</param>
    /// <param name="grayscale">Line of Sight Blue Factor.</param>
    /// <param name="xMin">Left boundry of vision rectangle.</param>
    /// <param name="xMax">Right boundry of vision rectangle.</param>
    /// <param name="yMin">Top boundry of vision rectangle.</param>
    /// <param name="yMax">Bottom boundry of vision rectangle.</param>
    /// <param name="xiMin">Left boundry of entity base.</param>
    /// <param name="xiMax">Right boundry of entity base</param>
    /// <param name="yiMin">Top boundry of entity base</param>
    /// <param name="yiMax">Bottom boundry of entity base</param>
    private void RevealWithHeight(Rect rect, float range, float height, float los, float fow, float grayscale, int xMin, int xMax, int yMin, int yMax, int xiMin, int xiMax, int yiMin, int yiMax)
    {
        for (int a = 0; a < 4; ++a)
        {
            int d = (a % 2);

            // JK holds the vision rectangle
            // jMid/kMid holds the middle point of the entity.

            // jMin and jMax are temporary variables from xMin/yMin and xMax/yMax.
            int jMin = 0, jMax = 0;

            // jMin and jMax are temporary variables from xMin/yMin and xMax/yMax.
            int kMin = 0, kMax = 0;

            // jMid and kMid are temporary variables to hold the midpoint from this j and k rectangle.
            int jMid = 0, kMid = 0;

            if (d == 0)
            {
                // This iteration jMin and jMax are used to xMin and xMax (x boundries).
                jMin = xMin;
                jMax = xMax;

                // This iteration kMin and kMax are used to yMin and yMax (y boundries).
                kMin = yMin;
                kMax = yMax;

                // jMid is the Width middle point, and kMid the Height middle point.
                // jMid = Width / 2
                // kMid = Height / 2
                jMid = (xiMin + xiMax) / 2;
                kMid = (yiMin + yiMax) / 2;
            }
            else
            {
                // This iteration jMin and jMax are used to yMin and yMax (y boundries).
                jMin = yMin;
                jMax = yMax;

                // This iteration kMin and kMax are used to xMin and xMax (x boundries).
                kMin = xMin;
                kMax = xMax;

                // jMid is the Height middle point, and kMid the Width middle point.
                // jMid = Height / 2
                // kMid = Width / 2
                jMid = (yiMin + yiMax) / 2;
                kMid = (xiMin + xiMax) / 2;
            }

            //?
            int prevMax = 0;

            for (int dj = jMin - jMid; dj <= jMax - jMid; ++dj)
            {
                int kEnd = (a < 2 ? kMax - kMid : kMid - kMin);

                int kStart = (a < 2 ? Mathf.Max(kMin - kMid, 0) : Mathf.Max(kMid - kMax, 0));

                if (kEnd <= 0) continue;

                for (int dk = kStart; dk <= kEnd; ++dk)
                {
                    jCache[dk] = -1000;
                }

                int curMax = kEnd;

                for (int dk = kStart; dk <= kEnd; ++dk)
                {
                    int wj = jMid + dj * dk / kEnd;

                    if (jCache[dk] >= dj) continue;

                    jCache[dk] = dj;

                    int wk = kMid + dk * (a < 2 ? 1 : -1);

                    int wx = d == 0 ? wj : wk;

                    int wy = d == 0 ? wk : wj;

                    if (wx < 0 || wy < 0 || wx >= losTexture.width || wy >= losTexture.height) continue;

                    if (curMax == kEnd && dk >= 1 && (wx < xiMin || wy < yiMin || wx > xiMax || wy > yiMax))
                    {
                        if (blockHeights[wy, wx] > height) curMax = dk;
                    }
                    {
                        var nodePos = new Vector2(wx, wy) / Scale;

                        var intlPos = new Vector2(
                            Mathf.Clamp(nodePos.x, rect.xMin, rect.xMax - 1),
                            Mathf.Clamp(nodePos.y, rect.yMin, rect.yMax - 1)
                        );

                        float dist2 = (intlPos - nodePos).sqrMagnitude / (range * range);

                        if (dist2 > 1) continue;

                        float bright = 1;

                        const float FadeStart = 0.8f;

                        if (dist2 > FadeStart * FadeStart)
                        {
                            bright = Mathf.Clamp01((1 - Mathf.Sqrt(dist2)) / (1 - FadeStart));
                        }

                        int p = wx + wy * losTexture.width;

                        if (dk > curMax) bright = bright * (0.75f - 0.5f * (dk - curMax) / 3);

                        pixels[p].r = (byte)Mathf.Max(pixels[p].r, (byte)(bright * los));
                        pixels[p].g = (byte)Mathf.Max(pixels[p].g, (byte)(bright * fow));
                        pixels[p].b = (byte)Mathf.Max(pixels[p].b, (byte)(Mathf.Clamp(bright * grayscale, 0, 255)));

                    }
                    if (dk > curMax + 1)
                    {
                        if (dk >= prevMax) break;
                    }
                }
                prevMax = curMax;
            }
        }
    }

    // Get the extents of a point/rectangle
    private void GetExtents(Vector2 pos, int inflateRange, out int xMin, out int yMin, out int xMax, out int yMax)
    {
        xMin = Mathf.RoundToInt(pos.x - inflateRange);
        xMax = Mathf.RoundToInt(pos.x + inflateRange);
        yMin = Mathf.RoundToInt(pos.y - inflateRange);
        yMax = Mathf.RoundToInt(pos.y + inflateRange);
        if (xMin < 0) xMin = 0; else if (xMax >= losTexture.width) xMax = losTexture.width - 1;
        if (yMin < 0) yMin = 0; else if (yMax >= losTexture.height) yMax = losTexture.height - 1;
    }

    /// <summary>
    /// Get calculated left, top, right, bottom coordinate from an rectangle.
    /// Rect is transformed to FoW Texture coordinate and scale, then it's xMin, xMax, yMin and yMax are validated inside the FoW Texture.
    /// Then, are setted to out variables to be used properly.
    /// </summary>
    /// <param name="rect">Rectangle to be validated.</param>
    /// <param name="inflateRange">Inflation range.</param>
    /// <param name="xMin">Valid left coordinate of the rectangle.</param>
    /// <param name="yMin">Valid top coordinate of the rectangle.</param>
    /// <param name="xMax">Valid right coordinate of the rectangle.</param>
    /// <param name="yMax">Valid bottom coordinate of the rectangle.</param>
    private void GetExtents(Rect rect, int inflateRange, out int xMin, out int yMin, out int xMax, out int yMax)
    {
        // Scale left and right coordinate.
        xMin = Mathf.RoundToInt(rect.xMin * Scale) - inflateRange;
        xMax = Mathf.RoundToInt(rect.xMax * Scale) + inflateRange;

        // Scale top and bottom coordiante.
        yMin = Mathf.RoundToInt(rect.yMin * Scale - 1) - inflateRange;
        yMax = Mathf.RoundToInt(rect.yMax * Scale - 1) + inflateRange;

        // Validate if left coordinate is inside valid position (bigger than 0)
        if (xMin < 0)
        {
            xMin = 0;
        }
        // Validate if right coordiante is inside valid position (inside FoW Texture widht).
        else if (xMax >= losTexture.width)
        {
            xMax = losTexture.width - 1;
        }

        // Validate if top coordinate is inside valid position (bigger than 0)
        if (yMin < 0)
        {
            yMin = 0;
        }
        // Validate if bottom coordiante is inside valid position (inside FoW Texture widht).
        else if (yMax >= losTexture.height)
        {
            yMax = losTexture.height - 1;
        }

        // If right coordinate is less than left coordinate, we should consider right coordinate equals to left coordinate.
        // The same bottom and top.
        if (xMax < xMin) xMax = xMin;
        if (yMax < yMin) yMax = yMin;
    }

    /// <summary>
    /// Add ambient occlusion around an area of unity.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="height"></param>
    private void AddAO(Rect rect, float height)
    {
        byte aoAmount = (byte)AOIntensity;
        byte nonAOAmount = (byte)(255 - aoAmount);
        float spreadRange = height / 2 + 0.5f;
        int spreadRangeI = Mathf.RoundToInt(spreadRange * Scale);
        int xMin, yMin, xMax, yMax;
        GetExtents(rect, spreadRangeI, out xMin, out yMin, out xMax, out yMax);
        for (int y = yMin; y <= yMax; ++y)
        {
            float yIntl = Mathf.Clamp(y / Scale, rect.yMin, rect.yMax - 1);
            for (int x = xMin; x <= xMax; ++x)
            {
                var nodePos = new Vector2(x, y) / Scale;
                var intlPos = new Vector2(Mathf.Clamp(nodePos.x, rect.xMin, rect.xMax - 1), yIntl);
                float dst2 = (intlPos - nodePos).sqrMagnitude;
                if (dst2 >= spreadRange * spreadRange) continue;
                int p = x + y * losTexture.width;
                //byte value = (byte)Mathf.Clamp(128 + (spreadAmnt) * dst2, 0, 255);
                byte value = (byte)(nonAOAmount + aoAmount *
                    2 * dst2 / (dst2 * 1 + spreadRange * spreadRange + 1)
                );
                if (pixels[p].a > value) pixels[p].a = value;
            }
        }
    }

    /// <summary>
    /// Add entity to be handled by the FoW Manager.
    /// </summary>
    /// <param name="entity"></param>
    public static void AddEntity(LOSEntity entity)
    {
        if (Instance != null && !Instance.Entities.Contains(entity)) Instance.Entities.Add(entity);
    }

    /// <summary>
    /// Remove entity from FoW manager.
    /// </summary>
    /// <param name="entity"></param>
    public static void RemoveEntity(LOSEntity entity)
    {
        if (Instance != null) Instance.Entities.Remove(entity);
    }
}
