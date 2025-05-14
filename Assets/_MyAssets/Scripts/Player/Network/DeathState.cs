using Fusion;
using UnityEngine;
using GorillaLocomotion;
using UnityEngine.InputSystem;

public class DeathState : NetworkBehaviour
{
    [Header("Channels")]
    [SerializeField] private NetworkPlayerChannel networkPlayerChannel;

    [Header("Ragdoll Setup")]
    [SerializeField] private Rigidbody[] ragdollBodies;
    [SerializeField] private Collider[] ragdollColliders;

    [Header("Objects To Hide After Death")]
    [SerializeField] private GameObject[] objectsToHide;

    private Transform playerTransform;

    private void Awake() => playerTransform = Player.Instance.transform.parent;

    public void DeathSequence()
    {
        SetRagdollState(true);
        if (!Object.HasStateAuthority) return;

        SimulationSync.Instance.ScheduleCallback(nameof(HidePlayerVisuals), this, 2f);
        SimulationSync.Instance.ScheduleCallback(nameof(RespawnAndReset), this, 5f);
    }

    public void HidePlayerVisuals()
    {
        foreach (var go in objectsToHide)
            go.SetActive(false);
    }

    public void RespawnAndReset()
    {
        if (Object.HasStateAuthority)
        {
            networkPlayerChannel.InvokeOnPlayerDeath();
            var team = networkPlayerChannel.GetPlayerTeam();
            var spawnPoint = SpawnPoints.Instance.GetRandomSpawnPoint(team);
            playerTransform.SetLocalPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            Player.Instance.InitializeValues();
        }

        foreach (var go in objectsToHide)
            go.SetActive(true);

        SetRagdollState(false);
        networkPlayerChannel.InvokeOnPlayerRespawned();
    }

    private void SetRagdollState(bool active)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !active;
            rb.detectCollisions = active;
            rb.useGravity = active;
        }

        foreach (var col in ragdollColliders)
            col.enabled = active;
    }
}
