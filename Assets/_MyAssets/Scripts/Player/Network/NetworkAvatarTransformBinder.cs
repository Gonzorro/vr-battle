using Fusion;
using UnityEngine;

public class NetworkAvatarTransformBinder : NetworkBehaviour
{
    [Header("Player Channel")]
    [SerializeField] private NetworkPlayerChannel playerChannel;

    [Header("Network Avatar Transforms")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private void Start()
    {
        if (!Object.HasStateAuthority) return;

        var transforms = new PlayerNetworkTransforms
        {
            Head = head,
            Body = body,
            LeftHand = leftHand,
            RightHand = rightHand
        };

        playerChannel.SetPlayerNetworkTransforms(transforms);
    }
}
