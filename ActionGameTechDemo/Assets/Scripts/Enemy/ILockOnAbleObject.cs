using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Damageable part of an enemy. Allows customised lockon AND customised damageTaken behaviour
/// </summary>
public interface ILockOnAbleObject
{
    public Transform LockOnTarget { get; }
}
