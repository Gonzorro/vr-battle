using UnityEngine;

public class AvatarRootFollower : MonoBehaviour
{
    [Header("Player Channel")]
    [SerializeField] private NetworkPlayerChannel playerChannel;

    [Header("Offset")]
    [SerializeField] private Vector3 positionOffset;

    private Transform body;
    private bool isReady;

    private void Update()
    {
        if (!isReady)
        {
            body = playerChannel.GetBody();
            isReady = true;
        }

        transform.SetPositionAndRotation(body.position + positionOffset, body.rotation);
    }
}
