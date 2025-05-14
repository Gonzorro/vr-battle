using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public struct PlayerData : INetworkStruct
{
    public NetworkString<_16> playerName;
    public NetworkBool isMasterClient;
}

public class NetworkPlayerRegistry : NetworkBehaviour, IStateAuthorityChanged
{
    [Header("Channels")]
    [SerializeField] private NetworkRunnerChannel networkRunnerChannel;

    [Networked, Capacity(20)] private NetworkDictionary<PlayerRef, PlayerData> PlayersRegistry { get; }

    private void Awake() => networkRunnerChannel.RequestMasterClientPlayerRef = () =>
                                     TryGetMasterClient(out var master) ? master : (PlayerRef?)null;

    public override void Spawned()
    {
        networkRunnerChannel.OnPlayerJoined += HandlePlayerJoined;
        networkRunnerChannel.OnPlayerLeft += HandlePlayerLeft;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        networkRunnerChannel.OnPlayerJoined -= HandlePlayerJoined;
        networkRunnerChannel.OnPlayerLeft -= HandlePlayerLeft;
    }

    private void HandlePlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient || !Object.HasStateAuthority) return;

        var isMaster = (player == runner.LocalPlayer);

        var data = new PlayerData
        {
            playerName = $"Player {player.PlayerId}",
            isMasterClient = isMaster
        };

        if (!PlayersRegistry.ContainsKey(player))
            PlayersRegistry.Set(player, data);
    }

    private void HandlePlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient || !Object.HasStateAuthority) return;

        if (PlayersRegistry.ContainsKey(player))
            PlayersRegistry.Remove(player);
    }

    public void StateAuthorityChanged()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        if (PlayersRegistry.TryGet(Runner.LocalPlayer, out var data))
        {
            data.isMasterClient = true;
            PlayersRegistry.Set(Runner.LocalPlayer, data);
        }

        UpdateRegistry();
    }

    private void UpdateRegistry()
    {
        foreach (var player in Runner.ActivePlayers)
        {
            if (!PlayersRegistry.ContainsKey(player))
            {
                var data = new PlayerData
                {
                    playerName = $"Player {player.PlayerId}",
                    isMasterClient = false
                };

                PlayersRegistry.Set(player, data);
            }
        }

        var keysToRemove = new List<PlayerRef>();
        foreach (var entry in PlayersRegistry)
        {
            if (!Runner.ActivePlayers.Contains(entry.Key))
                keysToRemove.Add(entry.Key);
        }

        foreach (var staleKey in keysToRemove)
            PlayersRegistry.Remove(staleKey);
    }

    public bool TryGetPlayer(PlayerRef playerRef, out PlayerData data) =>
        PlayersRegistry.TryGet(playerRef, out data);

    public bool TryGetMasterClient(out PlayerRef masterClient)
    {
        foreach (var playerData in PlayersRegistry)
        {
            if (playerData.Value.isMasterClient)
            {
                masterClient = playerData.Key;
                return true;
            }
        }

        masterClient = default;
        return false;
    }

    public int GetPlayerCount()
    {
        int count = PlayersRegistry.Count;
        return count > 0 ? count : 1;
    }

    private void LogAllPlayers(string context)
    {
        string playerListLog = $"PlayersRegistry ({context}):\n";
        foreach (var entry in PlayersRegistry)
        {
            playerListLog += $"- {entry.Key}: {entry.Value.playerName}, IsMaster: {entry.Value.isMasterClient}    ";
        }

        Debug.LogError(playerListLog);
    }
}
