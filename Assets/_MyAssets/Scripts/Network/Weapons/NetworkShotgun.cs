using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkShotgun : NetworkGunBase
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
        if (Time.time - lastFireTime < fireDelay || currentAmmo <= 0) return;

        lastFireTime = Time.time;
        currentAmmo--;
        RPC_Fire();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Fire()
    {
        if (firePoint == null || projectilePrefab == null) return;

        var shell = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);
        if (shell.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}
