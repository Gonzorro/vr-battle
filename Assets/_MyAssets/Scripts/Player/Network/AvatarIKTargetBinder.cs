using UnityEngine;
using DitzelGames.FastIK;

public class AvatarIKTargetBinder : MonoBehaviour
{
    [Header("Player Channel")]
    [SerializeField] private NetworkPlayerChannel playerChannel;

    [Header("FastIK Components")]
    [SerializeField] private FastIKFabric leftHandIK;
    [SerializeField] private FastIKFabric rightHandIK;
    [SerializeField] private FastIKFabric headIK;

    private void Awake()
    {
        leftHandIK.Target = playerChannel.GetLeftHand();
        rightHandIK.Target = playerChannel.GetRightHand();
        headIK.Target = playerChannel.GetHead();
    }
}
