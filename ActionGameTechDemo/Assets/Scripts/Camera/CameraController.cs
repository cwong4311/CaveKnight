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

    public static CameraController instance;

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

    private void Awake()
    {
        instance = this;
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
        _lookAngle += (mouseX * LookSpeed) / delta;
        _pivotAngle -= (mouseY * PivotSpeed) / delta;
        _pivotAngle = Mathf.Clamp(_pivotAngle, MinPivot, MaxPivot);

        transform.rotation = Quaternion.Euler(Vector3.up * _lookAngle);
        PivotTransform.localRotation = Quaternion.Euler(Vector3.right * _pivotAngle);
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
}
