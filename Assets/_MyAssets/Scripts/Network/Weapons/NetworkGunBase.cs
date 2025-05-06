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
    [SerializeField] protected bool isAutomatic = false;
    [SerializeField] protected bool usePhysicsForce = true;

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

    protected virtual void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log($"{name} grabbed by {args.interactorObject.transform.name}");
    }

    protected virtual void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log($"{name} released by {args.interactorObject.transform.name}");
    }

    protected virtual void OnActivated(ActivateEventArgs args)
    {
        Debug.Log($"{name} activated");
    }

    protected virtual void OnDeactivated(DeactivateEventArgs args)
    {
        Debug.Log($"{name} deactivated");
    }
}
