using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface ISingleSelector
{
    void HandleSelection(Vector3 selectionPosition, int buttonIndex);
}