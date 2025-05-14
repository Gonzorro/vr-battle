using System;
using Fusion;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum Team
{
    Red,
    Blue
}

public enum FlagState
{
    AtBase,
    Held,
    Away
}

public class FlagController : NetworkBehaviour
{
    [Header("Channels")]
    [SerializeField] private NetworkPlayerChannel networkPlayerChannel;

    [Header("References")]
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private NetworkRunnerChannel networkRunnerChannel;

    [Header("Flag Setup")]
    [SerializeField] private Team flagTeam;
    [SerializeField] private BoxCollider boxCollider;

    [Header("Runtime")]
    [Networked] public FlagState CurrentState { get; private set; }
    [Networked] public PlayerRef Holder { get; private set; }
    [Networked] public Team HolderTeam { get; private set; }

    public Team FlagTeam { get => flagTeam; set => flagTeam = value; }

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnDropped);

        Debug.LogError($"Team: {networkPlayerChannel.GetPlayerTeam()}");
    }

    public override void Spawned() => transform.SetParent(null);

    private void OnGrabbed(SelectEnterEventArgs enterEvent)
    {
        if (!Object.HasStateAuthority)
            Object.RequestStateAuthority();

        var playerTeam = networkPlayerChannel.GetPlayerTeam();

        if (Runner.IsSharedModeMasterClient)
            GrabFlag(Runner.LocalPlayer, playerTeam);
        else
        {
            var master = networkRunnerChannel.RequestMasterClientPlayerRef?.Invoke();
            if (master.HasValue)
                RPC_RequestGrabFlag(master.Value, Runner.LocalPlayer, playerTeam);
        }
    }

    private void OnDropped(SelectExitEventArgs exitEvent)
    {
        if (Runner.IsSharedModeMasterClient)
            DropFlag();
        else
        {
            var master = networkRunnerChannel.RequestMasterClientPlayerRef?.Invoke();
            if (master.HasValue)
                RPC_RequestDropFlag(master.Value);
            else
                Debug.LogError("Master Client not found!");
        }
    }

    public void GrabFlag(PlayerRef grabbingPlayer, Team grabbingTeam)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        Holder = grabbingPlayer;
        HolderTeam = grabbingTeam;
        CurrentState = FlagState.Held;

        Debug.LogError("Flag grabbed " + flagTeam + " by team " + grabbingTeam);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestDropFlag(PlayerRef requester)
    {
        if (!Runner.IsSharedModeMasterClient || requester != networkRunnerChannel.RequestMasterClientPlayerRef?.Invoke()) return;

        DropFlag();
    }

    public void DropFlag()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        Holder = PlayerRef.None;
        CurrentState = FlagState.Away;
        //boxCollider.enabled = false;
        //boxCollider.enabled = true; 

        Debug.LogError("Flag dropped " + flagTeam);
    }

    public void ReturnToBase()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        RPC_ForceUngrab(Holder);
        Holder = PlayerRef.None;
        CurrentState = FlagState.AtBase;
        transform.SetPositionAndRotation(originalPosition, originalRotation);

        Debug.LogError("Flag returned " + flagTeam);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ForceUngrab(PlayerRef target)
    {
        ForceDrop();
        Debug.LogError("RPC ForceUngrab triggered by: " + target.PlayerId);
    }

    private void ForceDrop()
    {
        grabInteractable.enabled = false;
        grabInteractable.enabled = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        if (Holder == PlayerRef.None && CurrentState != FlagState.AtBase)
            CurrentState = FlagState.Away;
    }

    public void ResetFlag() => ReturnToBase();

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestGrabFlag(PlayerRef requester, PlayerRef grabbingPlayer, Team grabbingTeam)
    {
        if (!Runner.IsSharedModeMasterClient || requester != networkRunnerChannel.RequestMasterClientPlayerRef?.Invoke()) return;

        GrabFlag(grabbingPlayer, grabbingTeam);
    }
}
