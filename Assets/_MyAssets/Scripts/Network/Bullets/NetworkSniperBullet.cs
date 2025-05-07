using UnityEngine;

public class NetworkSniperBullet : NetworkBulletBase
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource impactAudioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;

        int otherLayer = 1 << collision.gameObject.layer;

        if ((environmentLayer & otherLayer) != 0)
            impactAudioSource.Play();

        if ((environmentLayer | playerLayer & otherLayer) != 0)
        {
            ToggleVisualsAndCollider(false);
            StartCoroutine(DespawnAfter(1f));
        }
    }
}
