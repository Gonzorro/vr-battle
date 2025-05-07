using UnityEngine;

public class NetworkShotgunShell : NetworkBulletBase
{
    [Header("Pellet Spread")]
    [SerializeField] private Transform[] pellets;
    [SerializeField] private float spreadSpeed = 2f;

    private Vector3[] originalPositions;
    private float spawnTime;

    private void Awake() => SaveOriginalPositions();

    protected override void OnEnable() => base.OnEnable();

    public override void Spawned()
    {
        base.Spawned();
        spawnTime = Time.time;
    }

    private void OnDisable() => SetToOriginalPosition();

    protected override void Update()
    {
        base.Update();

        float delta = Time.deltaTime;
        foreach (var pellet in pellets)
            pellet.localPosition += delta * spreadSpeed * pellet.localPosition.normalized;

        if (!isReady && !Object.HasStateAuthority) return;
        if (Time.time - spawnTime > lifeTime)
            Runner.Despawn(Object);
    }

    private void SaveOriginalPositions()
    {
        originalPositions = new Vector3[pellets.Length];
        for (int i = 0; i < pellets.Length; i++)
            originalPositions[i] = pellets[i].localPosition;
    }

    private void SetToOriginalPosition()
    {
        if (originalPositions == null || pellets == null) return;
        for (int i = 0; i < pellets.Length; i++)
            pellets[i].localPosition = originalPositions[i];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;

        int otherLayer = 1 << collision.gameObject.layer;

        if ((environmentLayer | playerLayer & otherLayer) != 0)
            Runner.Despawn(Object);
    }
}
