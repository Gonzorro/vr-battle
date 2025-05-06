using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReloadUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkGunBase networkGunBase;
    [SerializeField] private Image reloadImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] reloadClips;

    private Coroutine reloadRoutine;

    private void Awake()
    {
        networkGunBase.OnReloading += OnReloading;
        SetAlpha(0);
    }

    private void OnDestroy() => networkGunBase.OnReloading -= OnReloading;

    private void OnReloading(float delay)
    {
        if (reloadRoutine != null)
            StopCoroutine(reloadRoutine);

        reloadRoutine = StartCoroutine(ReloadSequence(delay));
    }

    private IEnumerator ReloadSequence(float delay)
    {
        PlayReloadSound(delay);

        Color startColor = Color.red;
        Color endColor = Color.green;

        reloadImage.fillAmount = 0f;
        SetAlpha(0f);
        yield return FadeAlpha(1f, 0.1f);

        float timer = 0f;
        while (timer < delay)
        {
            float t = timer / delay;
            reloadImage.fillAmount = t;
            reloadImage.color = Color.Lerp(startColor, endColor, t);
            timer += Time.deltaTime;
            yield return null;
        }

        reloadImage.fillAmount = 1f;
        reloadImage.color = endColor;

        yield return FadeAlpha(0f, 0.2f);
        reloadRoutine = null;
    }

    private void PlayReloadSound(float reloadDuration)
    {
        if (reloadClips.Length == 0 || audioSource == null) return;

        var clip = reloadClips[Random.Range(0, reloadClips.Length)];

        float pitch = clip.length > 0f ? clip.length / reloadDuration : 1f;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }

    private IEnumerator FadeAlpha(float targetAlpha, float duration)
    {
        float startAlpha = reloadImage.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = reloadImage.color;
        c.a = alpha;
        reloadImage.color = c;
    }
}
