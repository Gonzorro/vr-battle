using Fusion;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class NetworkSniperBullet : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip impactAudioClip;
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioSource impactAudioSource;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Colliders")]
    [SerializeField] private BoxCollider boxCollider;

    [Header("Visuals")]
    [SerializeField] private GameObject visuals;

    private void OnEnable()
    {
        ToggleVisualsAndPhysics(true);
        shootAudioSource.pitch = Random.Range(0.95f, 1.05f);
        shootAudioSource.PlayOneShot(shootAudioClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        int otherLayerMask = 1 << other.gameObject.layer;

        if ((environmentLayer & otherLayerMask) != 0)
        {
            if (impactAudioSource && impactAudioClip)
            {
                impactAudioSource.transform.position = other.ClosestPoint(transform.position);
                impactAudioSource.pitch = Random.Range(0.95f, 1.05f);
                impactAudioSource.PlayOneShot(impactAudioClip);
            }
        }

        if ((environmentLayer | playerLayer & otherLayerMask) != 0)
        {
            ToggleVisualsAndPhysics(false);
            StartCoroutine(DelayedDespawn(1f));
        }
    }

    private IEnumerator DelayedDespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        Runner.Despawn(Object);
    }

    private void ToggleVisualsAndPhysics(bool isActive)
    {
        visuals.SetActive(isActive);
        boxCollider.enabled = isActive;
    }
}
