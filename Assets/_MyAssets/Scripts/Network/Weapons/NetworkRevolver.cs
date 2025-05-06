using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkRevolver : NetworkGunBase
{
    private int currentAmmo;

    protected override void OnGrabbed(SelectEnterEventArgs args)
    {
        base.OnGrabbed(args);
        currentAmmo = maxAmmo;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        TryFire();
    }

    private void TryFire()
    {
        if (Time.time - lastFireTime < fireDelay || currentAmmo <= 0)
            return;

        lastFireTime = Time.time;
        currentAmmo--;

        RPC_Fire();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Fire()
    {
        if (firePoint == null || projectilePrefab == null)
            return;

        var projectile = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);
        if (projectile.TryGetComponent<Rigidbody>(out var rb) && usePhysicsForce)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }
    }
}
