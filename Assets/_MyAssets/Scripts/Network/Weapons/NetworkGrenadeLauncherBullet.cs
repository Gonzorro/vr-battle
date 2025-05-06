using Fusion;
using UnityEngine;
using System.Collections;

public class NetworkGrenadeLauncherBullet : NetworkBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float damage = 10f;

    [Header("References")]
    [SerializeField] private GameObject visuals;
    [SerializeField] private Rigidbody pinRigidbody;
    [SerializeField] private Transform pinOriginalParent;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip launchClip;
    [SerializeField] private AudioClip pinClip;

    [Header("VFX")]
    [SerializeField] private GrenadeVFXHandler vfxHandler;
    [SerializeField] private ParticleSystem muzzleFx;

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

    private void OnDisable()
    {
        RestorePin();
        muzzleFx.Stop();
    }

    private void OnEnable()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(launchClip);
        muzzleFx.Play();
        visuals.SetActive(true);
    }

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

        vfxHandler.PlayAndDetach();
        IsIgniting = false;
        visuals.SetActive(false);

        yield return new WaitForSeconds(2f);
        if (Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    private void ReleasePin()
    {
        if (!pinRigidbody) return;

        audioSource.PlayOneShot(pinClip);

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
