using Fusion;
using UnityEngine;
using System.Collections;

public abstract class NetworkBulletBase : NetworkBehaviour
{
    [Header("Common Settings")]
    [SerializeField] protected float lifeTime = 3f;
    [SerializeField] private int damage = 10;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireSound;

    [Header("Visuals & Collision")]
    [SerializeField] protected GameObject visuals;
    [SerializeField] protected Collider bulletCollider;

    [Header("Collision Layers")]
    [SerializeField] protected LayerMask environmentLayer;
    [SerializeField] protected LayerMask playerLayer;

    private float despawnTime;
    protected bool isReady;

    public int Damage { get => damage; set => damage = value; }

    public override void Spawned()
    {
        despawnTime = Time.time + lifeTime;
        OnBulletSpawned();
        isReady = true;
    }

    protected virtual void OnEnable()
    {
        ToggleVisualsAndCollider(true);
        PlayClip(fireSound, audioSource, 0.95f, 1.05f);
    }

    protected virtual void Update()
    {
        if (!isReady) return;

        if (Object.HasStateAuthority && Time.time >= despawnTime)
            StartCoroutine(DespawnAfter(1f));
    }

    protected void PlayClip(AudioClip clip, AudioSource source, float pitchMin, float pitchMax)
    {
        source.pitch = Random.Range(pitchMin, pitchMax);
        source.PlayOneShot(clip);
    }

    protected void ToggleVisualsAndCollider(bool state)
    {
        visuals.SetActive(state);
        bulletCollider.enabled = state;
    }

    protected IEnumerator DespawnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    protected virtual void OnBulletSpawned() { }
}
