using UnityEngine;
using System.Collections;

public class SelectionBox : BaseMonoBehaviour
{
    /// <summary>
    /// See if a box was created while dragging.
    /// </summary>
    public bool Dragged
    {
        get
        {
            return selection.width > 0 && selection.height > 0;
        }
    }

    /// <summary>
    /// Resulting selection box.
    /// </summary>
    public Rect Selection
    {
        get;
        private set;
    }

    /// <summary>
    /// Rectangle created by the selection box, used for calculations.
    /// </summary>
    private Rect selection = new Rect(0, 0, 0, 0);

    /// <summary>
    /// Starting click, used for calculations.
    /// </summary>
    private Vector3 startClick = -Vector3.one;

    /// <summary>
    /// Color of the selection box.
    /// </summary>
    public Color AreaColor;

    /// <summary>
    /// Texture used to make the box.
    /// </summary>
    Texture2D BoxTexture;

    void Awake()
    {
        CreateTexture();
    }

    private void CreateTexture()
    {
        BoxTexture = new Texture2D(1, 1);
        BoxTexture.SetPixel(0, 0, AreaColor);
        BoxTexture.Apply();
    }

    void Update()
    {
        HandleSelection();
    }

    void OnGUI()
    {
        if (selection.width == 0 || selection.height == 0) return;
        GUI.DrawTexture(selection, BoxTexture);
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startClick = Input.mousePosition;
            selection.x = startClick.x;
            selection.y = InvertY(startClick.y);
        }

        if (Input.GetMouseButton(0))
        {
            float width = Input.mousePosition.x - startClick.x;
            float height = InvertY(Input.mousePosition.y) - InvertY(startClick.y);

            if (width < 0)
            {
                selection.x = Input.mousePosition.x;
                width = -width;
            }
            else
            {
                selection.x = startClick.x;
            }

            if (height < 0)
            {
                selection.y = InvertY(Input.mousePosition.y);
                height = -height;
            }
            else
            {
                selection.y = InvertY(startClick.y);
            }

            selection.width = width;
            selection.height = height;

            Selection = selection;
        }

        if (Input.GetMouseButtonUp(0))
        {
            selection = new Rect(0, 0, 0, 0);
        }
    }
}