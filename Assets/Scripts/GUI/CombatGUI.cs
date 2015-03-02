using UnityEngine;
using System.Collections;

public class CombatGUI : MonoBehaviour
{
    void OnGUI()
    {
        string message = (CombatManager.Instance.OnCombat) ? "Enabled" : "Disabled";
        GUI.Label(new Rect(Screen.width/2, 0, 200, 30), "Combat: " + message);
    }
}
