using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private NetworkRunnerChannel networkRunnerChannel;

    [Header("Prefabs")]
    [SerializeField] private GameObject networkRunnerPrefab;

    [Header("Settings")]
    [SerializeField] private string roomName = "testRoom";

    private GameMode gameMode = GameMode.Shared;
    private NetworkRunner networkRunner;
    private GameObject activeRunnerInstance;
    private bool isConnected;

    private void OnEnable() => networkRunnerChannel.OnRunnersChanged += OnRunnersReady;

    private void OnDisable() => networkRunnerChannel.OnRunnersChanged -= OnRunnersReady;

    private void Start() => SpawnRunner();

    private void SpawnRunner()
    {
        isConnected = false;
        if (activeRunnerInstance) Destroy(activeRunnerInstance);
        activeRunnerInstance = Instantiate(networkRunnerPrefab);
    }

    private void OnRunnersReady(NetworkRunner runner, NetworkSceneManagerDefault sceneManager, INetworkObjectProvider provider)
    {
        if (runner == null || isConnected) return;

        networkRunner = runner;
        StartGame(sceneManager, provider);
    }

    private async void StartGame(NetworkSceneManagerDefault sceneManager, INetworkObjectProvider provider)
    {
        if (!TryGetSceneRef(out var sceneRef))
        {
            Debug.LogError("Invalid scene reference. Cannot start the Room.");
            return;
        }

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Scene = sceneRef,
            SessionName = roomName,
            PlayerCount = 10,
            SceneManager = sceneManager,
            ObjectProvider = provider
        });

        if (!result.Ok)
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            return;
        }

        isConnected = true;
    }

    private bool TryGetSceneRef(out SceneRef sceneRef)
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        if (index < 0 || index >= SceneManager.sceneCountInBuildSettings)
        {
            sceneRef = default;
            return false;
        }

        sceneRef = SceneRef.FromIndex(index);
        return true;
    }
}
