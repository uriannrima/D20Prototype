using UnityEngine;
using System.Collections;

/// <summary>
/// What is this entity state of revealing.
/// </summary>
public enum RevealStates { Hidden, Fogged, Unfogged, };

public class LOSEntity : MonoBehaviour
{
    // Some cache parameters for FOW animation
    Color _oldfowColor = Color.clear;

    // Fow color cache for interpolation.
    Color _fowColor = Color.clear;

    /// <summary>
    /// Current state of this entity, if it's visible, fogged, or hidden.
    /// </summary>
    public RevealStates RevealState;

    /// <summary>
    /// Range of View of this entity.
    /// </summary>
    public float Range = 10;

    /// <summary>
    /// FoW Interpolation
    /// </summary>
    float _fowInterp = 1;

    /// <summary>
    /// Should this entity reveal the surrouding area on the fog?
    /// </summary>
    public bool IsRevealer = false;

    /// <summary>
    /// Ambient Occlusion only really works on static entities, or very tall entities
    /// </summary>
    public bool EnableAO = false;

    /// <summary>
    /// Height of the unity. 
    /// Used for AO and peering (peaking) over walls.
    /// </summary>
    public float Height = 1;

    /// <summary>
    /// Base of the entity.
    /// </summary>
    public float BaseSize = 0;

    /// <summary>
    /// Entity bounds on the terrain
    /// </summary>
    public virtual Rect Bounds
    {
        get
        {
            return new Rect(
                transform.position.x - BaseSize / 2,
                transform.position.z - BaseSize / 2,
                BaseSize, BaseSize);;
        }
    }

    /// <summary>
    /// If the FoW Interpolation is less than 1, we need to update it.
    /// </summary>
    public bool RequiresFOWUpdate
    {
        get
        {
            return _fowInterp < 1;
        }
    }

    /// <summary>
    /// Change actual fow color of this unity.
    /// </summary>
    /// <param name="fowColor">Target fow color.</param>
    /// <param name="interpolate">Should interpolate?</param>
    public void SetFOWColor(Color fowColor, bool interpolate)
    {
        fowColor.a = 255;
        if (fowColor == _fowColor) return;
        if (!interpolate)
        {
            _fowColor = fowColor;
            _fowInterp = 1;
            UpdateFOWColor();
            return;
        }
        _oldfowColor = Color.Lerp(_oldfowColor, _fowColor, _fowInterp);
        _fowColor = fowColor;
        _fowInterp = 0;
        UpdateFOWColor();
    }

    /// <summary>
    /// Update FoW Color for this entity, and tell if we still need to update it (animate it).
    /// </summary>
    /// <returns>True if it's not updating. False if it finished update it's fow color.</returns>
    public bool UpdateFOWColor()
    {
        // Calculate FoW Interpolation
        _fowInterp = Mathf.Clamp01(_fowInterp + Time.deltaTime / 0.4f);

        // Interpolate the old and destionation color.
        var fowColor = Color.Lerp(_oldfowColor, _fowColor, _fowInterp);

        // For each renderer inside this object.
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            // If actual color Red or Green isn't completelly black (0).
            if (fowColor.r > 0 || fowColor.g > 0)
            {
                // Set its _FOWColor Material parameter to it's actual FoW Color.
                foreach (var material in renderer.materials)
                {
                    material.SetColor("_FOWColor", fowColor);
                }
            }
        }

        return !RequiresFOWUpdate;
    }

    public void SetIsCurrentTeam(bool isCurrent)
    {
        IsRevealer |= isCurrent;
    }

    // Tell the LOS manager that we're here
    public void OnEnable()
    {
        LOSManager.AddEntity(this);
    }

    public void OnDisable()
    {
        LOSManager.RemoveEntity(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0, 0, 0.5F);
        Gizmos.DrawCube(transform.position, new Vector3(BaseSize, Height, BaseSize));
    }
}
