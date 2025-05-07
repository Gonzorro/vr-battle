using UnityEngine;
using Fusion;

public class AvatarSpawner : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private NetworkRunnerChannel runnerChannel;
    [SerializeField] private NetworkPlayerChannel playerChannel;

    [Header("Avatar Prefab")]
    [SerializeField] private NetworkObject avatarPrefab;

    private void OnEnable() => runnerChannel.OnConnectedToServer += HandleConnected;

    private void OnDisable() => runnerChannel.OnConnectedToServer -= HandleConnected;

    private void HandleConnected(NetworkRunner runner) => SpawnAvatar(runner);

    private void SpawnAvatar(NetworkRunner runner)
    {
        Vector3 spawnPosition = transform.position;
        runner.Spawn(avatarPrefab, spawnPosition, Quaternion.identity);
    }
}
