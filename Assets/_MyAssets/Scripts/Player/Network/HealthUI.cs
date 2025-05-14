using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private NetworkPlayerHealth playerHealth;
    [SerializeField] private Image fillImage;

    private void OnEnable() => playerHealth.OnHealthChangedEvent += UpdateFill;
    private void OnDisable() => playerHealth.OnHealthChangedEvent -= UpdateFill;

    private void UpdateFill(int health)
    {
        float percent = health / 100f;
        fillImage.fillAmount = percent;
        fillImage.color = Color.Lerp(Color.red, Color.green, percent);
    }
}
