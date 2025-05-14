using Fusion;
using UnityEngine;

public class NetworkPlayerInfo : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkPlayerChannel networkPlayerChannel;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Team Materials")]
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material redMaterial;

    public override void Spawned()
    {
        SetPlayerColor();

        if (!Object.HasStateAuthority) return;

        if (Runner.LocalPlayer.PlayerId == 1)
            networkPlayerChannel.SetPlayerTeam(Team.Red);
        else
            networkPlayerChannel.SetPlayerTeam(Team.Blue);
    }

    private void SetPlayerColor()
    {
        var materials = skinnedMeshRenderer.materials;
        materials[0] = Object.StateAuthority.PlayerId == 1 ? redMaterial : blueMaterial;
        skinnedMeshRenderer.materials = materials;
    }

    public Team GetPlayerTeam() => networkPlayerChannel.GetPlayerTeam();
}
