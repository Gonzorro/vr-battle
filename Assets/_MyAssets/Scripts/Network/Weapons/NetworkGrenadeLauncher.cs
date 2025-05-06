using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrenadeLauncher : NetworkGunBase
{
    private int currentAmmo;
    private NetworkObject currentGrenade;

    protected override void OnGrabbed(SelectEnterEventArgs args)
    {
        base.OnGrabbed(args);
        currentAmmo = maxAmmo;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        TryLaunch();
    }

    protected override void OnDeactivated(DeactivateEventArgs args) => Explode();

    private void TryLaunch()
    {
        if (Time.time - lastFireTime < fireDelay || currentAmmo <= 0) return;
        lastFireTime = Time.time;
        currentAmmo--;
        Fire();
    }

    private void Fire()
    {
        currentGrenade = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);
        if (currentGrenade.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * projectileSpeed;
    }

    private void Explode()
    {
        if (currentGrenade == null) return;
        Runner.Despawn(currentGrenade);
        currentGrenade = null;
    }
}
