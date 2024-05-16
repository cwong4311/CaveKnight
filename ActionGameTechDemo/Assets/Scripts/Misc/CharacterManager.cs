using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterManager : MonoBehaviour, ILockOnAbleObject
{

    public Transform LockOnTransform;

    public Transform LockOnTarget
    {
        get
        {
            if (LockOnTransform == null)
            {
                return transform;
            }
            else
            {
                return LockOnTransform;
            }
        }
    }

    public abstract void TriggerHitStop(float damageAmount, bool isAttacker);
}
