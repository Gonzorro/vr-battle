using UnityEngine;

public class NetworkPistolBullet : NetworkBulletBase
{
    protected override void Update() => base.Update();

    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;
        Debug.LogError($"Bullet collided with: {collision.gameObject.name}");
        int otherLayer = 1 << collision.gameObject.layer;

        if ((environmentLayer | playerLayer & otherLayer) != 0)
            Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("Skipped trigger: no state authority");
            return;
        }

        int otherLayer = 1 << other.gameObject.layer;
        Debug.LogError($"Trigger with: {other.gameObject.name}, Layer: {other.gameObject.layer}");

        if ((playerLayer & otherLayer) != 0)
        {
            Debug.LogError("Hit a player layer object");

            if (other.TryGetComponent<NetworkPlayerHealth>(out var health))
            {
                Debug.LogError("Found NetworkPlayerHealth component, applying damage");
                health.DamagePlayer(Mathf.RoundToInt(Damage));
            }
            else
            {
                Debug.LogError("No NetworkPlayerHealth found on player-layer object");
            }
        }

        if ((environmentLayer & otherLayer) != 0 || (playerLayer & otherLayer) != 0)
        {
            Debug.LogError("Despawn bullet after hitting environment or player");
            Runner.Despawn(Object);
        }
    }
}
