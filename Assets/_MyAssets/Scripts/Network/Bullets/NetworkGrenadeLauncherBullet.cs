using Fusion;
using UnityEngine;
using System.Collections;

public class NetworkGrenadeLauncherBullet : NetworkBulletBase
{
    [Header("Pin References")]
    [SerializeField] private Rigidbody pinRigidbody;
    [SerializeField] private Transform pinOriginalParent;
    [SerializeField] private AudioClip pinClip;

    private Vector3 pinLocalPosition;
    private Quaternion pinLocalRotation;
    private Vector3 pinLocalScale;

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
        IsIgniting = false;
        ToggleVisualsAndCollider(false);

        yield return new WaitForSeconds(2f);
        if (Object.HasStateAuthority)
            Runner.Despawn(Object);
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
