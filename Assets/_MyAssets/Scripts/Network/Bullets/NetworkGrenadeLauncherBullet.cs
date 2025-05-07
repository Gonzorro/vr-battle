using Fusion;
using UnityEngine;
using System.Collections;

public class NetworkGrenadeLauncherBullet : NetworkBulletBase
{
    [Header("Pin References")]
    [SerializeField] private Rigidbody pinRigidbody;
    [SerializeField] private Transform pinOriginalParent;
    [SerializeField] private AudioClip pinClip;

    [Header("Explosion Collision Check")]
    [SerializeField] private LayerMask collisionCheckMask;
    [SerializeField] private float collisionCheckRadius = 20f;

    private Vector3 pinLocalPosition;
    private Quaternion pinLocalRotation;
    private Vector3 pinLocalScale;

    private readonly Collider[] _overlapHits = new Collider[10];

    [Networked, OnChangedRender(nameof(OnIsIgnitingChanged))] private NetworkBool IsIgniting { get; set; }

    private void Awake()
    {
        pinLocalPosition = pinRigidbody.transform.localPosition;
        pinLocalRotation = pinRigidbody.transform.localRotation;
        pinLocalScale = pinRigidbody.transform.localScale;
    }

    private void OnDisable() => RestorePin();

    public void Ignite()
    {
        if (!Object.HasStateAuthority) return;
        IsIgniting = true;
    }

    private void OnIsIgnitingChanged()
    {
        if (!IsIgniting) return;

        ReleasePin();
        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        yield return new WaitForSeconds(1f);

        ParticleManager.Instance.Play(ParticleType.LauncherExplosion, transform.position);
       // CheckRadiusForLayer();

        IsIgniting = false;
        ToggleVisualsAndCollider(false);

        yield return new WaitForSeconds(2f);
        if (Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    //TODO not working
    private void CheckRadiusForLayer()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, collisionCheckRadius, _overlapHits, collisionCheckMask);

        for (int i = 0; i < count; i++)
        {
            var hit = _overlapHits[i];
            Debug.LogError(hit.gameObject.name);

            if ((collisionCheckMask.value & (1 << hit.gameObject.layer)) == 0) continue;

            if (hit.TryGetComponent<TargetScript>(out var target))
                target.IsHit = true;
        }
    }


    private void ReleasePin()
    {
        if (!pinRigidbody) return;

        PlayClip(pinClip, audioSource, 1f, 1f);

        pinRigidbody.transform.SetParent(null);
        pinRigidbody.isKinematic = false;

        Vector3 upwardRandom = (Vector3.up + Random.insideUnitSphere * 0.5f).normalized;
        pinRigidbody.AddForce(upwardRandom * 3.5f, ForceMode.Impulse);
    }

    private void RestorePin()
    {
        pinRigidbody.isKinematic = true;
        pinRigidbody.transform.SetParent(pinOriginalParent);
        pinRigidbody.transform.SetLocalPositionAndRotation(pinLocalPosition, pinLocalRotation);
        pinRigidbody.transform.localScale = pinLocalScale;
    }
}
