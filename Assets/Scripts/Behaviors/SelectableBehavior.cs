using UnityEngine;
using System.Collections;

public class SelectableBehavior : BaseBehavior
{
    SelectionMark SelectionMark;

    void Start()
    {
        SelectionMark = gameObject.GetComponentInChild<SelectionMark>();
        SelectionMark.Color = GetComponent<CharacterData>().Color;
    }

    public void Select()
    {
        SelectionMark.Select();
    }

    public void Deselect()
    {
        SelectionMark.Deselect();
    }

    public void PreSelect()
    {
        SelectionMark.PreSelect();
    }


}
