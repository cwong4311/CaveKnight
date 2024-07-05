using UnityEngine;


public class FollowTransform : MonoBehaviour
{
    public Transform FollowTarget;

    public void Update()
    {
        if (FollowTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        this.transform.position = FollowTarget.position;
    }
}