using UnityEngine;
using System.Collections;

public class SelectionMark : BaseMonoBehaviour
{
    /// <summary>
    /// Same color for all instances of Selection Mark.
    /// </summary>
    public static Color PreColor = Color.white;

    /// <summary>
    /// Each Selection Mark has it's own selection color.
    /// </summary>
    public Color Color = Color.gray;

    public void PreSelect()
    {
        this.gameObject.SetActive(true);
        ChangeColor(PreColor);
    }

    public void Select()
    {
        this.gameObject.SetActive(true);
        ChangeColor(Color);
    }

    public void Deselect()
    {
        this.gameObject.SetActive(false);
    }
}
