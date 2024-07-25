using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects any collision on Colliders on the same GO as this component, and adds them to a list
/// </summary>
public class CollisionDetector : MonoBehaviour
{
    protected List<Collider> _colliders = new List<Collider>();

    public List<Collider> Colliders => _colliders;

    public void OnCollisionEnter(Collision collision)
    {
        var collider = collision.collider;
        if (IsHandleCollisionType(collision) && _colliders.Contains(collider) == false)
        {
            _colliders.Add(collider);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        var collider = collision.collider;
        if (_colliders.Contains(collider))
        {
            _colliders.Remove(collider);
        }
    }

    public virtual bool IsHandleCollisionType(Collision collision)
    {
        return true;
    }
}
