using Fusion;
using UnityEngine;

public class NetworkShotgunShell : NetworkBehaviour
{
    [SerializeField] private Transform[] pellets;
    [SerializeField] private float spreadSpeed = 2f;
    [SerializeField] private float lifetime = 2f;

    private Vector3[] originalPositions;
    private float spawnTime;
    private bool isReady;

    private void Awake() => SaveOriginalPosition();

    public override void Spawned()
    {
        spawnTime = Time.time;
        isReady = true;
    }

    private void Update()
    {
        if (!isReady) return;

        float delta = Time.deltaTime;
        foreach (var pellet in pellets)
            pellet.localPosition += delta * spreadSpeed * pellet.localPosition.normalized;

        if (!Object.HasStateAuthority) return;
        if (Time.time - spawnTime > lifetime)
            Runner.Despawn(Object);
    }

    private void SaveOriginalPosition()
    {
        originalPositions = new Vector3[pellets.Length];
        for (int i = 0; i < pellets.Length; i++)
            originalPositions[i] = pellets[i].localPosition;
    }

    private void OnDisable()
    {
        if (originalPositions == null || pellets == null) return;
        for (int i = 0; i < pellets.Length; i++)
            pellets[i].localPosition = originalPositions[i];
    }
}
