using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReloadUIController : MonoBehaviour
{
    [Header("XR & Gun References")]
    [SerializeField] private XRGrabInteractable xRGrabInteractable;
    [SerializeField] private NetworkGunBase networkGunBase;

    [Header("UI Components")]
    [SerializeField] private Image reloadImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private GameObject ammoPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] reloadClips;


    private Coroutine reloadRoutine;

    private void Awake()
    {
        xRGrabInteractable.selectEntered.AddListener(OnSelectEntered);
        xRGrabInteractable.selectExited.AddListener(OnSelectExited);

        networkGunBase.OnReloading += OnReloading;
        networkGunBase.OnAmmoChanged += UpdateAmmoText;
    }

    private void Start() => Init();

    private void OnDestroy()
    {
        xRGrabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        xRGrabInteractable.selectExited.RemoveListener(OnSelectExited);

        networkGunBase.OnReloading -= OnReloading;
        networkGunBase.OnAmmoChanged -= UpdateAmmoText;
    }

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        UpdateAmmoText(networkGunBase.CurrentAmmo, networkGunBase.MaxAmmo);
        ammoPanel.SetActive(true);
    }

    private void OnSelectExited(SelectExitEventArgs arg0) => ammoPanel.SetActive(false);

    private void Init()
    {
        SetAlpha(0);
        UpdateAmmoText(networkGunBase.MaxAmmo, networkGunBase.MaxAmmo);
        ammoPanel.SetActive(false);
    }
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

    private void UpdateAmmoText(int current, int max)
    {
        string currentText = current == 0 ? $"<color=red>{current}</color>" : current.ToString();
        ammoText.text = $"{currentText} / {max}";
    }
}
