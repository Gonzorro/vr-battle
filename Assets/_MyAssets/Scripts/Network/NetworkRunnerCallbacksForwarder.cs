using Fusion;
using System;
using UnityEngine;
using Fusion.Sockets;
using System.Collections.Generic;

public class NetworkRunnerCallbacksForwarder : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Channels")]
    [SerializeField] private NetworkRunnerChannel networkRunnerChannel;

    private NetworkRunner NetworkRunner;

    private void Awake() => networkRunnerChannel.OnRunnersChanged += OnRunnersChanged;

    private void OnDestroy() => networkRunnerChannel.OnRunnersChanged -= OnRunnersChanged;

    private void OnRunnersChanged(NetworkRunner runner, NetworkSceneManagerDefault sceneManager)
    {
        NetworkRunner = runner; 
        NetworkRunner.AddCallbacks(this);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) => networkRunnerChannel.InvokePlayerJoined(runner, player);

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) => networkRunnerChannel.InvokePlayerLeft(runner, player);

    public void OnConnectedToServer(NetworkRunner runner) => networkRunnerChannel.InvokeConnectedToServer(runner);

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) => networkRunnerChannel.InvokeDisconnectedFromServer(runner, reason);

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) =>
        networkRunnerChannel.InvokeOnServerShutdown(runner, reason);
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
