using UnityEngine;
using System.Collections;
using RAIN.Core;
using RAIN.Navigation.Pathfinding;

public class MovementBehavior : BaseBehavior
{
    /// <summary>
    /// 
    /// </summary>
    public string MemoryItemName;

    /// <summary>
    /// 
    /// </summary>
    AttackBehavior AttackBehavior;

    void Start()
    {
        SetMemory<Vector3>(MemoryItemName, this.transform.position);
        AttackBehavior = GetComponent<AttackBehavior>();
    }

    public void Move(Vector3 position)
    {
        RAINPath path = null;
        if (this.AIRig.AI.Navigator.GetPathTo(position, 100, false, out path))
        {
            if (AttackBehavior)
            {
                AttackBehavior.StopAttack();
            }

            SetMemory<Vector3>(MemoryItemName, position);
            SetMemory<bool>("isMoving", true);
            WaypointManager.Instance.CreateWaypoint(position, this.gameObject, GetComponent<CharacterData>().Color);
        }
    }

}
