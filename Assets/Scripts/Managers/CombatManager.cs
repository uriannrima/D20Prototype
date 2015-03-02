using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : BaseManager<CombatManager>
{
    public bool OnCombat = false;

    public List<GameObject> Combatants = new List<GameObject>();
}
