using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

/// Handles player gravity only when not grounded, and stepping over short objects.
public class PlayerGravity : MonoBehaviour
{
    [Header("Gravity")]
    public float CharacterGravity;
    public float TerminalFallSpeed;
    public float FallAcceleration;

    [Header("Steps")]
    public float maxStepHeight = 0.4f;         
    public float stepSearchOvershoot = 0.01f;       

    private List<ContactPoint> allCPs = new List<ContactPoint>();
    private Vector3 lastVelocity;

    private Rigidbody _rb;
    private float gravityRamp = 0f;
    private float accelerationRamp = 0f;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();

        gravityRamp = CharacterGravity;
        accelerationRamp = FallAcceleration;
    }

    void FixedUpdate()
    {
        Vector3 velocity = _rb.velocity;

        //Filter through the ContactPoints to see if we're grounded and to see if we can step up
        ContactPoint groundCP = default(ContactPoint);
        bool grounded = FindGround(out groundCP, allCPs);

        Vector3 stepUpOffset = default(Vector3);
        bool stepUp = false;
        if (grounded)
        {
            stepUp = FindStep(out stepUpOffset, allCPs, groundCP, velocity);
            gravityRamp = CharacterGravity;
            accelerationRamp = FallAcceleration;
        }
        else
        {
            // Only apply gravity when feet are not touching the ground
            accelerationRamp *= 1.1f;
            gravityRamp = Mathf.SmoothStep(gravityRamp, TerminalFallSpeed, Time.fixedDeltaTime * accelerationRamp);
            _rb.velocity += (Vector3.down * gravityRamp) * Time.fixedDeltaTime;
        }
  
        //Steps
        if (stepUp)
        {
            _rb.position += stepUpOffset;
            _rb.velocity = lastVelocity;
        }

        allCPs.Clear();
        lastVelocity = velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }

    void OnCollisionStay(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }

    bool FindGround(out ContactPoint groundCP, List<ContactPoint> allCPs)
    {
        groundCP = default(ContactPoint);
        bool found = false;
        foreach (ContactPoint cp in allCPs)
        {
            //Pointing with some up direction
            if (cp.normal.y > 0.0001f && (found == false || cp.normal.y > groundCP.normal.y))
            {
                groundCP = cp;
                found = true;
            }
        }

        return found;
    }

    /// Find the first step up point if we hit a step
    bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> allCPs, ContactPoint groundCP, Vector3 currVelocity)
    {
        stepUpOffset = default(Vector3);

        //No chance to step if the player is not moving
        Vector2 velocityXZ = new Vector2(currVelocity.x, currVelocity.z);
        if (velocityXZ.sqrMagnitude < 0.0001f)
            return false;

        foreach (ContactPoint cp in allCPs)
        {
            bool stepUp = ResolveStepUp(out stepUpOffset, cp, groundCP);
            if (stepUp)
            {
                return stepUp;
            }
                
        }
        return false;
    }

    /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
    bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);
        Collider stepCol = stepTestCP.otherCollider;

        // If the collision point is specifically tagged unclimable or is an enemy, don't even evaluate
        if (stepCol.gameObject.CompareTag("Unclimable") || stepCol.gameObject.CompareTag("Enemy"))
        {
            return false;
        }

        // Check if the contact point normal matches that of a step (y close to 0)
        if (Mathf.Abs(stepTestCP.normal.y) >= 0.5f)
        {
            return false;
        }

        // Make sure the contact point is low enough to be a step
        if (stepTestCP.point.y - groundCP.point.y > maxStepHeight)
        {
            return false;
        }

        // Check to see if there's actually a place to step in front of us
        // Fires one Raycast
        RaycastHit hitInfo;
        float stepHeight = groundCP.point.y + maxStepHeight + 0.0001f;
        Vector3 stepTestInvDir = new Vector3(-stepTestCP.normal.x, 0, -stepTestCP.normal.z).normalized;
        Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 direction = Vector3.down;
        if (!(stepCol.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight)))
        {
            return false;
        }

        // Calculate the point
        Vector3 stepUpPoint = new Vector3(stepTestCP.point.x, hitInfo.point.y + 0.0001f, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCP.point.x, groundCP.point.y, stepTestCP.point.z);

        // Return the result
        stepUpOffset = stepUpPointOffset;
        return true;
    }
}