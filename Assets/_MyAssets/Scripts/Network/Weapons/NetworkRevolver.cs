using UnityEngine;

public class NetworkRevolver : NetworkGunBase
{
    protected override void Fire()
    {
        var projectile = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}
