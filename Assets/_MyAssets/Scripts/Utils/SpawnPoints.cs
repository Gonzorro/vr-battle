using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static SpawnPoints Instance { get; private set; }

    [Header("Red Team Spawn Points")]
    [SerializeField] private Transform[] redSpawnPoints;

    [Header("Blue Team Spawn Points")]
    [SerializeField] private Transform[] blueSpawnPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Transform[] GetSpawnPoints(Team team) =>
        team == Team.Red ? redSpawnPoints : blueSpawnPoints;

    public Transform GetRandomSpawnPoint(Team team)
    {
        var points = GetSpawnPoints(team);
        return points[Random.Range(0, points.Length)];
    }

}
