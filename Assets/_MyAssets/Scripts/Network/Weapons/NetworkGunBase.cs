using System;
using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkGunBase : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected NetworkObject projectilePrefab;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float fireDelay = 0.5f;
    [SerializeField] protected float reloadTime = 1f;
    [SerializeField] protected float projectileSpeed = 25f;

    protected float lastFireTime;
    private bool isReloading;

    [Networked] protected int CurrentAmmo { get; set; }

    private XRGrabInteractable grabInteractable;
    public event Action<float> OnReloading;

    private void Awake() => grabInteractable = GetComponent<XRGrabInteractable>();

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            CurrentAmmo = maxAmmo;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        grabInteractable.activated.AddListener(OnActivated);
        grabInteractable.deactivated.AddListener(OnDeactivated);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
        grabInteractable.activated.RemoveListener(OnActivated);
        grabInteractable.deactivated.RemoveListener(OnDeactivated);
    }

    protected virtual void OnGrabbed(SelectEnterEventArgs args) => Debug.Log($"[NetworkGunBase] Grabbed by {args.interactorObject}");
    protected virtual void OnReleased(SelectExitEventArgs args) => Debug.Log($"[NetworkGunBase] Released by {args.interactorObject}");
    protected virtual void OnActivated(ActivateEventArgs args)
    {
        Debug.Log($"[NetworkGunBase] Activated by {args.interactorObject}");
        TryFire();
    }

    protected virtual void OnDeactivated(DeactivateEventArgs args) => Debug.Log($"[NetworkGunBase] Deactivated by {args.interactorObject}");

    protected void TryFire()
    {
        if (isReloading || Time.time - lastFireTime < fireDelay || CurrentAmmo <= 0) return;

        lastFireTime = Time.time;
        CurrentAmmo--;
        Fire();

        if (CurrentAmmo <= 0)
        {
            TriggerReload();
            Invoke(nameof(ReloadAmmo), reloadTime);
        }
    }

    protected virtual void Fire() { }

    private void ReloadAmmo()
    {
        if (!Object.HasStateAuthority) return;

        CurrentAmmo = maxAmmo;
        isReloading = false;
    }

    protected void TriggerReload()
    {
        if (isReloading) return;
        isReloading = true;
        OnReloading?.Invoke(reloadTime);
    }

    public void ForceReloadNow()
    {
        CancelInvoke(nameof(ReloadAmmo));
        ReloadAmmo();
    }
}
