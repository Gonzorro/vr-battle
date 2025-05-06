using UnityEngine;

public class NetworkSniper : NetworkGunBase
{
    protected override void Fire()
    {
        var projectile = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}
