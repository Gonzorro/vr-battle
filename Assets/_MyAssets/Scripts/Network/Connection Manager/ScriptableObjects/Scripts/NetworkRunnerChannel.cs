using System;
using Fusion;
using UnityEngine;
using Fusion.Sockets;

[CreateAssetMenu(fileName = "NetworkRunnerChannel", menuName = "Fusion/NetworkRunnerChannel")]
public class NetworkRunnerChannel : ScriptableObject
{
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private NetworkEvents networkEvents;
    [SerializeField] private NetworkSceneManagerDefault networkSceneManager;

    public event Action<NetworkRunner, NetworkSceneManagerDefault, INetworkObjectProvider> OnRunnersChanged;
    public event Action<NetworkRunner, PlayerRef> OnPlayerJoined;
    public event Action<NetworkRunner, PlayerRef> OnPlayerLeft;
    public event Action<NetworkRunner> OnConnectedToServer;
    public event Action<NetworkRunner, NetDisconnectReason> OnDisconnectedFromServer;
    public event Action<NetworkRunner, ShutdownReason> OnServerShutdown;

    public void UpdateRunners(NetworkRunner runner, NetworkSceneManagerDefault sceneManager, INetworkObjectProvider provider)
    {
        networkRunner = runner;
        networkSceneManager = sceneManager;
        OnRunnersChanged?.Invoke(networkRunner, networkSceneManager, provider);
    }

    public void InvokePlayerJoined(NetworkRunner runner, PlayerRef player) =>
        OnPlayerJoined?.Invoke(runner, player);

    public void InvokePlayerLeft(NetworkRunner runner, PlayerRef player) =>
        OnPlayerLeft?.Invoke(runner, player);

    public void InvokeConnectedToServer(NetworkRunner runner) => 
        OnConnectedToServer?.Invoke(runner);

    public void InvokeDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) =>
        OnDisconnectedFromServer?.Invoke(runner, reason);

    public void InvokeOnServerShutdown(NetworkRunner runner, ShutdownReason reason) =>
        OnServerShutdown?.Invoke(runner, reason);
}
