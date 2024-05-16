using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects any collision from GOs on a defined Layer with Colliders on this component, and adds them to a list
/// </summary>
public class LayerCollisionDetector : CollisionDetector
{
    public LayerMask LayerMask;

    public override bool IsHandleCollisionType(Collision collision)
    {
        var layer = collision.gameObject.layer;

        return LayerMask == (LayerMask | (1 << layer));
    }
}
