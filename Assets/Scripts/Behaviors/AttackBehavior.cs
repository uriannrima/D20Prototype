using UnityEngine;
using System.Collections;
using RAIN.Core;

public class AttackBehavior : BaseBehavior
{
    public GameObject AttackTarget;    

    public void Attack(GameObject target)
    {
        if (AttackTarget) AttackTarget.renderer.material.color = Color.white;

        AttackTarget = target;
        AttackTarget.renderer.material.color = Color.yellow;
    }

    public void StopAttack()
    {
        if (!AttackTarget) return;

        AttackTarget.renderer.material.color = Color.white;

        this.AttackTarget = null;
    }

    void Update()
    {
        if (AIRig && AttackTarget)
        {
            SetMemory<Vector3>("movePosition", AttackTarget.transform.position);
        }
    }

}
