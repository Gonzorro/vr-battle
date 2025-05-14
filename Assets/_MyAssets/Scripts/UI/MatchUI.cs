using TMPro;
using Fusion;
using UnityEngine;

public class MatchUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI redPointsText;
    [SerializeField] private TextMeshProUGUI bluePointsText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Look Down Activation")]
    [SerializeField] private float pitchThreshold = 0.3f;

    [Header("Lazy Follow Settings")]
    [SerializeField] private Vector3 offset = new(0, -0.35f, 0.5f);
    [SerializeField] private float followSpeed = 10f;

    private Transform headTransform;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            Destroy(gameObject);
            return;
        }

        headTransform = Camera.main.transform;
        transform.SetParent(null);

        NetworkGameManager.OnRedPointsUpdated += UpdateRedUI;
        NetworkGameManager.OnBluePointsUpdated += UpdateBlueUI;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        NetworkGameManager.OnRedPointsUpdated -= UpdateRedUI;
        NetworkGameManager.OnBluePointsUpdated -= UpdateBlueUI;
    }

    private void LateUpdate()
    {
        if (!Object.HasStateAuthority) return;

        float pitch = headTransform.rotation.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Deg2Rad * pitch;

        canvasGroup.alpha = pitch > pitchThreshold ? 1f : 0f;

        Vector3 targetPosition = headTransform.position + headTransform.rotation * offset;
        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed),
            Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - headTransform.position), Time.deltaTime * followSpeed)
        );
    }

    private void UpdateRedUI(int points) => redPointsText.text = points.ToString();
    private void UpdateBlueUI(int points) => bluePointsText.text = points.ToString();
}
