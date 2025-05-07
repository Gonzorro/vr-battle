using UnityEngine;

public class LocalAvatarTransformSync : MonoBehaviour
{
    [Header("Player Channel")]
    [SerializeField] private NetworkPlayerChannel playerChannel;

    [Header("Local Tracking Transforms")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform body;

    private Transform networkHeadTransform;
    private Transform networkLeftHandTransform;
    private Transform networkRightHandTransform;
    private Transform networkBodyTransform;

    private bool isReady;

    private void Awake() => playerChannel.OnPlayerNetworkTransformsReady += OnPlayerNetworkTransformsReady;

    private void OnDestroy() => playerChannel.OnPlayerNetworkTransformsReady -= OnPlayerNetworkTransformsReady;

    private void OnPlayerNetworkTransformsReady(PlayerNetworkTransforms transforms)
    {
        networkHeadTransform = transforms.Head;
        networkLeftHandTransform = transforms.LeftHand;
        networkRightHandTransform = transforms.RightHand;
        networkBodyTransform = transforms.Body;
        isReady = true;
    }

    private void Update()
    {
        if (!isReady) return;

        networkHeadTransform.SetPositionAndRotation(head.position, head.rotation);
        networkLeftHandTransform.SetPositionAndRotation(leftHand.position, leftHand.rotation);
        networkRightHandTransform.SetPositionAndRotation(rightHand.position, rightHand.rotation);
        networkBodyTransform.SetPositionAndRotation(body.position, body.rotation);
    }
}
