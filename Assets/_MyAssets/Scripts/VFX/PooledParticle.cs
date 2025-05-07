using UnityEngine;

public class PooledParticle : MonoBehaviour
{
    private AudioSource audioSource;
    private ParticleSystem[] systems;
    private float maxLifetime;
    private ParticleType type;

    public void SetType(ParticleType value) => type = value;

    private void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>(true);
        audioSource = GetComponent<AudioSource>();
        maxLifetime = 0f;

        foreach (var ps in systems)
        {
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            if (duration > maxLifetime)
                maxLifetime = duration;
        }
    }

    private void OnEnable()
    {
        PlaySound();

        foreach (var ps in systems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }

        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), maxLifetime);
    }

    private void PlaySound()
    {
        if (!audioSource || !audioSource.clip) return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        ParticleManager.Instance.ReturnToPool(type, this);
    }
}
