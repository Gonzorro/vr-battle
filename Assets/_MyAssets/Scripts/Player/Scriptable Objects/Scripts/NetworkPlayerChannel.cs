using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Network/PlayerChannelChannel")]
public class NetworkPlayerChannel : ScriptableObject
{
    public event Action<PlayerNetworkTransforms> OnPlayerNetworkTransformsReady;

    public void SetPlayerNetworkTransforms(PlayerNetworkTransforms transforms)
        => OnPlayerNetworkTransformsReady?.Invoke(transforms);
}

[Serializable]
public struct PlayerNetworkTransforms
{
    public Transform Head;
    public Transform Body;
    public Transform LeftHand;
    public Transform RightHand;
}
