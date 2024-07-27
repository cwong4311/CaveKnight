using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform TargetTransform;
    public Transform CameraTransform;
    public Transform PivotTransform;

    public Vector3 LockOnCameraOffset;
    private Vector3 CameraOrigin;

    private Vector3 _camPosition;
    private LayerMask _ignoreLayers;
    private Vector3 _camFollowVelocity = Vector3.zero;

    public float LookSpeed;
    public float FollowSpeed;
    public float PivotSpeed;
    public float MinPivot = -35;
    public float MaxPivot = 35;

    private float _targetPosition;
    private float _lookAngle;
    private float _pivotAngle;

    public float CameraSphereRadius = 0.2f;
    public float CameraCollisionOffset = 0.2f;
    public float MinCollisionOffset = 0.2f;

    public float maxLockonDistance = 30f;
    private List<ILockOnAbleObject> lockonTargets = new List<ILockOnAbleObject>();
    public Transform closestLockonTarget;
    public Transform currentLockonTarget;
    public int lockonIndex;

    private Coroutine _smoothLockMotion;

    public bool IsLockOnReady => _smoothLockMotion == null;

    private void Awake()
    {
        CameraOrigin = CameraTransform.localPosition;

        _ignoreLayers = ~(1 << 2 | 1 << 8 | 1 << 9 | 1 << 10);

        _lookAngle = transform.rotation.eulerAngles.y % 360;
        _smoothLockMotion = null;
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
        if (_smoothLockMotion != null) return;

        if (currentLockonTarget != null)
        {
            Vector3 dir = (currentLockonTarget.position - transform.position).normalized;
            dir.y = 0;
            var trackEnemyRotation = Quaternion.LookRotation(dir);
            _lookAngle = trackEnemyRotation.eulerAngles.y;
            transform.rotation = trackEnemyRotation;

            dir = (currentLockonTarget.position - PivotTransform.position).normalized;
            var eulerAngles = Quaternion.LookRotation(dir).eulerAngles;
            eulerAngles.y = 0;
            PivotTransform.localEulerAngles = eulerAngles;
            _pivotAngle = eulerAngles.x;
        }
        else
        {
            //_lookAngle += (mouseX * LookSpeed) / delta;
            _lookAngle = (_lookAngle + (Clamp(mouseX, -60, 60) * LookSpeed)) % 360;
            _pivotAngle -= (mouseY * PivotSpeed) / delta;
            _pivotAngle = Mathf.Clamp(_pivotAngle, MinPivot, MaxPivot);

            transform.rotation = Quaternion.Euler(Vector3.up * _lookAngle);
            PivotTransform.localRotation = Quaternion.Euler(Vector3.right * _pivotAngle);
        }
    }

    private void HandleCameraCollision(float delta)
    {
        _targetPosition = (currentLockonTarget != null) ? LockOnCameraOffset.z : CameraOrigin.z;

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
        if (TargetTransform == null) return false;

        float shortestDistance = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(TargetTransform.position, 30);

        for (int i = 0; i < colliders.Length; i++)
        {
            // Attempt to read a lock on object from this collider
            var lockonObj = colliders[i].GetComponent<ILockOnAbleObject>();
            if (lockonObj == null)
            {
                // If one cannot be found, try to search for a lock on redirect object
                var lockonRedirect = colliders[i].GetComponent<LockOnRedirectObject>();
                if (lockonRedirect != null)
                {
                    lockonObj = lockonRedirect.LockOnTarget;
                }
                // If neither exist, then this object cannot be locked onto
            }

            if (lockonObj != null)
            {
                if (lockonTargets.Contains(lockonObj)) continue;

                Vector3 lockonDir = lockonObj.LockOnTarget.position - TargetTransform.position;
                float distFromTarget = Vector3.Distance(TargetTransform.position, lockonObj.LockOnTarget.position);

                float viewAngle = Vector3.Angle(lockonDir, CameraTransform.forward);
                if (lockonObj.LockOnTarget.root != TargetTransform.transform.root
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
                                //hasCollision = true;
                            }
                        }
                    }

                    if (!hasCollision && lockonTargets.Contains(lockonObj) == false)
                    {
                        lockonTargets.Add(lockonObj);
                    }
                }
            }
        }

        for (int k = 0; k < lockonTargets.Count; k++)
        {
            float distFromTarget = Vector3.Distance(TargetTransform.position, lockonTargets[k].LockOnTarget.position);

            if (distFromTarget < shortestDistance)
            {
                shortestDistance = distFromTarget;
                closestLockonTarget = lockonTargets[k].LockOnTarget;
                lockonIndex = k;
            }
        }

        if (closestLockonTarget != null)
        {
            currentLockonTarget = closestLockonTarget;
            _smoothLockMotion = StartCoroutine(SmoothLockon());
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
        if (lockonTargets == null) return false;

        lockonTargets.RemoveAll(e => e.LockOnTarget == null);
        if (lockonTargets.Count <= 0) return false;

        var previousLockonTarget = currentLockonTarget;
        lockonIndex = (lockonIndex + 1) % lockonTargets.Count;
        currentLockonTarget = lockonTargets[lockonIndex].LockOnTarget;

        if (previousLockonTarget != currentLockonTarget)
        {
            _smoothLockMotion = StartCoroutine(SmoothLockon());
        }

        return true;
    }

    public void ClearLockon(bool forceSmoothUnlock = false)
    {
        // Only perform smooth unlock if clearing unlock while locked on
        // OR if force tag is set (ie Only LockOn target has been killed).
        if (currentLockonTarget != null || forceSmoothUnlock)
        {
            _smoothLockMotion = StartCoroutine(SmoothUnlock());
        }

        lockonTargets.Clear();
        currentLockonTarget = null;
        closestLockonTarget = null;
        lockonIndex = 0;
    }

    private float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private IEnumerator SmoothLockon()
    {
        float elapsedFixedDelta = 0;
        while (elapsedFixedDelta <= 0.5f)
        {
            if (currentLockonTarget == null) break;

            Vector3 dir = (currentLockonTarget.position - transform.position).normalized;
            dir.y = 0;
            var enemyRot = Quaternion.LookRotation(dir);

            dir = (currentLockonTarget.position - PivotTransform.position).normalized;
            var pivotEuler = Quaternion.LookRotation(dir).eulerAngles;
            pivotEuler.y = 0;

            // Make rotation slerp to lockon target's location
            transform.rotation = Quaternion.Slerp(transform.rotation, enemyRot, elapsedFixedDelta * 2);
            PivotTransform.localRotation = Quaternion.Slerp(PivotTransform.localRotation, Quaternion.Euler(pivotEuler), elapsedFixedDelta * 2);
            _lookAngle = transform.rotation.eulerAngles.y;
            _pivotAngle = PivotTransform.localRotation.eulerAngles.x;
            if (_pivotAngle > MaxPivot)
            {
                _pivotAngle = _pivotAngle - 360;
            }
            else if (_pivotAngle < MinPivot)
            {
                _pivotAngle = _pivotAngle + 360;
            }

            // Move camera smoothly to lockon position
            CameraTransform.localPosition = Vector3.Slerp(CameraTransform.localPosition, LockOnCameraOffset, elapsedFixedDelta * 2);

            _camPosition.x = CameraTransform.localPosition.x;
            _camPosition.y = CameraTransform.localPosition.y;

            elapsedFixedDelta += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _camPosition.x = LockOnCameraOffset.x;
        _camPosition.y = LockOnCameraOffset.y;
        _smoothLockMotion = null;
    }

    private IEnumerator SmoothUnlock()
    {
        bool resetPivot = false;

        float elapsedFixedDelta = 0;
        while (elapsedFixedDelta <= 0.33f)
        {
            // Move camera smoothly to origin
            CameraTransform.localPosition = Vector3.Slerp(CameraTransform.localPosition, CameraOrigin, elapsedFixedDelta * 3);
            if (_pivotAngle > MaxPivot || _pivotAngle < MinPivot)
            {
                resetPivot = true;

                var returnToZeroPivot = PivotTransform.localRotation.eulerAngles;
                returnToZeroPivot.x = 0;
                PivotTransform.localRotation = Quaternion.Slerp(PivotTransform.localRotation, Quaternion.Euler(returnToZeroPivot), elapsedFixedDelta * 3);
            }

            _camPosition.x = CameraTransform.localPosition.x;
            _camPosition.y = CameraTransform.localPosition.y;

            elapsedFixedDelta += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (resetPivot)
        {
            _pivotAngle = 0;
        }
        
        _camPosition.x = CameraOrigin.x;
        _camPosition.y = CameraOrigin.y;
        _smoothLockMotion = null;
    }
}
