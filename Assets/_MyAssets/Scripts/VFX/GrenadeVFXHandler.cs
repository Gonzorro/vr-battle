using System.Collections;
using UnityEngine;

public class GrenadeVFXHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] explosionClips;

    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private void Awake()
    {
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;
    }

    public void PlayAndDetach()
    {
        PlayAudioClip();

        transform.SetParent(null);

        foreach (var ps in particleSystems)
            ps.Play();

        StartCoroutine(ReturnAfterEffects());
    }

    private void PlayAudioClip()
    {
        var clip = explosionClips[Random.Range(0, explosionClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    private IEnumerator ReturnAfterEffects()
    {
        float maxDuration = 0f;

        foreach (var ps in particleSystems)
        {
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            if (duration > maxDuration)
                maxDuration = duration;
        }

        yield return new WaitForSeconds(maxDuration);

        transform.SetParent(originalParent);
        transform.SetLocalPositionAndRotation(originalLocalPosition, originalLocalRotation);
        transform.localScale = originalLocalScale;
    }
}
