using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IMultipleSelector
{
    void HandleSelection(Rect selectionBox, int buttonIndex, bool preSelection = false);
}