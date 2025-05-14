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

    [Header("Position Offsets")]
    [SerializeField] private Vector3 headPositionOffset;
    [SerializeField] private Vector3 leftHandPositionOffset;
    [SerializeField] private Vector3 rightHandPositionOffset;
    [SerializeField] private Vector3 bodyPositionOffset;

    private Transform networkHeadTransform;
    private Transform networkLeftHandTransform;
    private Transform networkRightHandTransform;
    private Transform networkBodyTransform;

    private bool isReady;

    private void Awake()
    {
        playerChannel.OnPlayerNetworkTransformsReady += OnPlayerNetworkTransformsReady;
        playerChannel.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnDestroy()
    {
        playerChannel.OnPlayerNetworkTransformsReady -= OnPlayerNetworkTransformsReady;
        playerChannel.OnPlayerDeath -= OnPlayerDeath;
    }

    private void OnPlayerNetworkTransformsReady(PlayerNetworkTransforms transforms)
    {
        networkHeadTransform = transforms.Head;
        networkLeftHandTransform = transforms.LeftHand;
        networkRightHandTransform = transforms.RightHand;
        networkBodyTransform = transforms.Body;
        isReady = true;
    }

    private void OnPlayerDeath() => isReady = false;

    private void Update()
    {
        if (!isReady) return;

        networkHeadTransform.SetPositionAndRotation(head.position + headPositionOffset, head.rotation);
        networkLeftHandTransform.SetPositionAndRotation(leftHand.position + leftHandPositionOffset, leftHand.rotation);
        networkRightHandTransform.SetPositionAndRotation(rightHand.position + rightHandPositionOffset, rightHand.rotation);
        networkBodyTransform.SetPositionAndRotation(body.position + bodyPositionOffset, body.rotation);
    }
}
