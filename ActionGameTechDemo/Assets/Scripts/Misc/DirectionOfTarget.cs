using UnityEngine;

public enum TargetDirection
{
    Forward,
    Backward,
    Left,
    Right
}

public class DirectionOfTarget
{
    public static TargetDirection GetDirectionOfTarget(Transform origin, Transform target)
    {
        Vector3 direction = target.position - origin.position;
        direction.y = 0; // Ignore vertical component

        direction.Normalize();

        // Calculate the angle between forward direction of A and direction to B
        float angle = Vector3.SignedAngle(origin.forward, direction, Vector3.up);

        if (angle >= -30 && angle <= 30)
        {
            return TargetDirection.Forward;
        }
        else if (angle > 30 && angle < 150)
        {
            return TargetDirection.Right;
        }
        else if (angle > -150 && angle < -30)
        {
            return TargetDirection.Left;
        }
        else
        {
            return TargetDirection.Backward;
        }
    }
}