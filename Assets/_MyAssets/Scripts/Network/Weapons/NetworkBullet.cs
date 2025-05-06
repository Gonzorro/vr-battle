using Fusion;
using UnityEngine;

public class NetworkBullet : NetworkBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 10f;

    private float despawnTime;

    public override void Spawned()
    {
        despawnTime = Time.time + lifeTime;
    }

    private void Update()
    {
        if (Time.time >= despawnTime && Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        Debug.Log($"Bullet hit: {other.name}");

        Runner.Despawn(Object);
    }
}
