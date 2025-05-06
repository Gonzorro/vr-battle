using UnityEngine;

public class NetworkGrenadeLauncher : NetworkGunBase
{
    private NetworkGrenadeLauncherBullet currentBullet;


    protected override void OnDeactivated(UnityEngine.XR.Interaction.Toolkit.DeactivateEventArgs args) => Ignite();

    protected override void Fire()
    {
        Quaternion randomRotation = GetRandomQuaternion();
        var spawnRotation = firePoint.rotation * randomRotation;

        var grenade = Runner.Spawn(projectilePrefab, firePoint.position, spawnRotation);

        if (grenade.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * projectileSpeed;

        grenade.TryGetComponent(out currentBullet);
    }

    private void Ignite()
    {
        if (currentBullet == null) return;

        currentBullet.Ignite();
        currentBullet = null;
    }

    private Quaternion GetRandomQuaternion() =>
        Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
}
