using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Network/PlayerChannelChannel")]
public class NetworkPlayerChannel : ScriptableObject
{
    private Team playerTeam;
    public event Action<PlayerNetworkTransforms> OnPlayerNetworkTransformsReady;

    public void SetPlayerNetworkTransforms(PlayerNetworkTransforms transforms)
        => OnPlayerNetworkTransformsReady?.Invoke(transforms);

    public void SetPlayerTeam(Team team) => playerTeam = team;
    public Team GetPlayerTeam() => playerTeam;

    public void InvokeOnPlayerDeath() => OnPlayerDeath?.Invoke();
    public event Action OnPlayerDeath;

    public void InvokeOnPlayerRespawned() => OnPlayerRespawned?.Invoke();
    public event Action OnPlayerRespawned;
}

[Serializable]
public struct PlayerNetworkTransforms
{
    public Transform Head;
    public Transform Body;
    public Transform LeftHand;
    public Transform RightHand;
}
