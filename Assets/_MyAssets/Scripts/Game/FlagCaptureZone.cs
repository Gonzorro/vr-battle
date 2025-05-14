using Fusion;
using UnityEngine;

public class FlagCaptureZone : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private Team baseTeam;
    [SerializeField] private FlagController ownFlag;
    [SerializeField] private FlagController enemyFlag;
    [SerializeField] private NetworkGameManager networkGameManager;

    private bool isReady;

    public override void Spawned() => isReady = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!isReady || !Runner.IsSharedModeMasterClient) return;
        if (!other.TryGetComponent(out FlagController flag)) return;

        if (flag.FlagTeam == baseTeam)
        {
            if (flag.HolderTeam == baseTeam && flag.CurrentState != FlagState.AtBase)
                flag.ReturnToBase();
            else if (flag.CurrentState == FlagState.Away)
                flag.ReturnToBase();
        }
        else if (
            ownFlag.CurrentState == FlagState.AtBase &&
            flag.CurrentState == FlagState.Held &&
            flag.HolderTeam == baseTeam
        )
        {
            networkGameManager.OnFlagCaptured(baseTeam == Team.Red);
            flag.ReturnToBase();
        }
    }
}
