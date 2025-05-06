using UnityEngine;

public class NetworkShotgun : NetworkGunBase
{
    protected override void Fire()
    {
        var shell = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);
        if (shell.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}
