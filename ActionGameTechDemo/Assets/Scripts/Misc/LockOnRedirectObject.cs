using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnRedirectObject : MonoBehaviour
{
    [SerializeField]
    private GameObject LockOnAbleObject;
    public ILockOnAbleObject LockOnTarget;

    public void OnEnable()
    {
        LockOnAbleObject.TryGetComponent(out LockOnTarget);
    }
}