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
    [SerializeField] protected float projectileSpeed = 25f;

    protected float lastFireTime;

    private XRGrabInteractable grabInteractable;

    private void Awake() => grabInteractable = GetComponent<XRGrabInteractable>();

    public override void Spawned()
    {
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

    protected virtual void OnGrabbed(SelectEnterEventArgs args) { }

    protected virtual void OnReleased(SelectExitEventArgs args) { }

    protected virtual void OnActivated(ActivateEventArgs args) { }

    protected virtual void OnDeactivated(DeactivateEventArgs args) { }
}
