using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Network/PlayerChannelChannel")]
public class NetworkPlayerChannel : ScriptableObject
{
    public Func<Transform> GetHead;
    public Func<Transform> GetBody;
    public Func<Transform> GetLeftHand;
    public Func<Transform> GetRightHand;
}
