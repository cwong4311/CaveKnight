using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform TargetTransform;
    public Transform CameraTransform;
    public Transform PivotTransform;

    private Vector3 _camPosition;
    private LayerMask _ignoreLayers;
    private Vector3 _camFollowVelocity = Vector3.zero;

    public float LookSpeed;
    public float FollowSpeed;
    public float PivotSpeed;
    public float MinPivot = -35;
    public float MaxPivot = 35;

    private float _targetPosition;
    private float _defaultPosition;
    private float _lookAngle;
    private float _pivotAngle;

    public float CameraSphereRadius = 0.2f;
    public float CameraCollisionOffset = 0.2f;
    public float MinCollisionOffset = 0.2f;

    public float maxLockonDistance = 30f;
    private List<CharacterManager> lockonTargets = new List<CharacterManager>();
    public Transform closestLockonTarget;
    public Transform currentLockonTarget;
    public int lockonIndex;

    private void Awake()
    {
        _defaultPosition = CameraTransform.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.SmoothDamp
        (
            transform.position, TargetTransform.position, ref _camFollowVelocity, delta / FollowSpeed
        );
        transform.position = targetPosition;

        HandleCameraCollision(delta);
    }

    public void HandleCameraRotation(float delta, float mouseX, float mouseY)
    {
        if (currentLockonTarget != null)
        {
            float velocity = 0;

            Vector3 dir = (currentLockonTarget.position - transform.position).normalized;
            dir.y = 0;

            transform.rotation = Quaternion.LookRotation(dir);

            dir = (currentLockonTarget.position - PivotTransform.position).normalized;
            var eulerAngles = Quaternion.LookRotation(dir).eulerAngles;
            eulerAngles.y = 0;
            PivotTransform.localEulerAngles = eulerAngles;
        }
        else
        {
            _lookAngle += (mouseX * LookSpeed) / delta;
            _pivotAngle -= (mouseY * PivotSpeed) / delta;
            _pivotAngle = Mathf.Clamp(_pivotAngle, MinPivot, MaxPivot);

            transform.rotation = Quaternion.Euler(Vector3.up * _lookAngle);
            PivotTransform.localRotation = Quaternion.Euler(Vector3.right * _pivotAngle);
        }
    }

    private void HandleCameraCollision(float delta)
    {
        _targetPosition = _defaultPosition;

        Vector3 dir = (CameraTransform.position - PivotTransform.position).normalized;
        if (Physics.SphereCast(PivotTransform.position, CameraSphereRadius, dir, out var hit, Mathf.Abs(_targetPosition), _ignoreLayers))
        {
            float dist = Vector3.Distance(PivotTransform.position, hit.point);
            _targetPosition = -(dist - CameraCollisionOffset);
        }

        if (Mathf.Abs(_targetPosition) < MinCollisionOffset)
        {
            _targetPosition = -MinCollisionOffset;
        }

        _camPosition.z = Mathf.Lerp(CameraTransform.localPosition.z, _targetPosition, delta / 0.2f);
        CameraTransform.localPosition = _camPosition;
    }

    public bool HandleLockon()
    {
        float shortestDistance = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(TargetTransform.position, 30);
        for (int i = 0; i < colliders.Length; i++)
        {
            var character = colliders[i].GetComponent<CharacterManager>();
            if (character != null)
            {
                Vector3 lockonDir = character.transform.position - TargetTransform.position;
                float distFromTarget = Vector3.Distance(TargetTransform.position, character.transform.position);

                float viewAngle = Vector3.Angle(lockonDir, CameraTransform.forward);
                if (character.transform.root != TargetTransform.transform.root
                    && viewAngle > -50 && viewAngle < 50
                    && distFromTarget <= maxLockonDistance)
                {
                    bool hasCollision = false;
                    RaycastHit[] hits = Physics.RaycastAll(TargetTransform.transform.position, lockonDir, distFromTarget);
                    if (hits != null)
                    {
                        foreach (var hit in hits)
                        {
                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
                            {
                                hasCollision = true;
                            }
                        }
                    }

                    if (!hasCollision)
                        lockonTargets.Add(character);
                }
            }
        }

        for (int k = 0; k < lockonTargets.Count; k++)
        {
            float distFromTarget = Vector3.Distance(TargetTransform.position, lockonTargets[k].transform.position);

            if (distFromTarget < shortestDistance)
            {
                shortestDistance = distFromTarget;
                closestLockonTarget = lockonTargets[k].transform;
                lockonIndex = k;
            }
        }

        if (closestLockonTarget != null)
        {
            currentLockonTarget = closestLockonTarget;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Cycles the lockon in order. If no more left to cycle, returns false
    /// </summary>
    /// <returns></returns>
    public bool CycleLockon()
    {
        if (lockonTargets == null || lockonTargets.Count <= 0) return false;

        lockonIndex = (lockonIndex + 1) % lockonTargets.Count;
        currentLockonTarget = lockonTargets[lockonIndex].transform;
        if (currentLockonTarget == closestLockonTarget)
        {
            return false;
        }

        return true;
    }

    public void ClearLockon()
    {
        lockonTargets.Clear();
        currentLockonTarget = null;
        closestLockonTarget = null;
        lockonIndex = 0;
    }
}
