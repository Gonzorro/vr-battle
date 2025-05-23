using Fusion;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private NetworkRunnerChannel channel;

    [Header("Runners")]
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkEvents networkEvents;
    [SerializeField] private NetworkSceneManagerDefault sceneManager;
    [SerializeField] private INetworkObjectProvider provider;

    private void OnEnable() => channel.UpdateRunners(runner, sceneManager, provider);
}
