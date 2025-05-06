using Fusion;
using UnityEngine;

public class NetworkBullet : NetworkBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    private float despawnTime;
    private bool isReady;
    public override void Spawned()
    {
        despawnTime = Time.time + lifeTime;
        isReady = true;
    }

    private void Update()
    {
        if (isReady && Time.time >= despawnTime && Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        Debug.Log($"Bullet hit: {other.name}");

        Runner.Despawn(Object);
    }
}
