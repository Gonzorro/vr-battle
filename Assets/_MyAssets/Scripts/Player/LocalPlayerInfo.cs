using UnityEngine;

public class LocalPlayerInfo : MonoBehaviour
{
    [Header("Player Channel")]
    [SerializeField] private NetworkPlayerChannel playerChannel;

    [Header("Local Transforms")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private void Awake()
    {
        playerChannel.GetHead = () => head;
        playerChannel.GetBody = () => body;
        playerChannel.GetLeftHand = () => leftHand;
        playerChannel.GetRightHand = () => rightHand;
    }
}
