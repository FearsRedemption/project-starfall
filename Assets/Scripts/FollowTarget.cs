using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    private void LateUpdate()
    {
        if (!target) return;
            transform.position = target.position + offset;
    }
}