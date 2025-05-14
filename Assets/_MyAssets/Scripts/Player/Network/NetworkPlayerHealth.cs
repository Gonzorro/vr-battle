using Fusion;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [Header("Channels")]
    [SerializeField] private NetworkPlayerChannel networkPlayerChannel;

    [Header("References")]
    [SerializeField] private DeathState deathState;

    [Header("Damage Settings")]
    [SerializeField] private LayerMask damageLayer;

    [Networked, OnChangedRender(nameof(OnHealthChanged))] private int Health { get; set; } = 100;

    public event Action<int> OnHealthChangedEvent;

    private void Awake() => networkPlayerChannel.OnPlayerRespawned += OnPlayerRespawned;

    private void OnDestroy() => networkPlayerChannel.OnPlayerRespawned -= OnPlayerRespawned;

    private void OnPlayerRespawned()
    {
        if (Object.HasStateAuthority)
            Health = 100;
    }

    public override void Spawned() => OnHealthChanged();

    private void OnHealthChanged() => OnHealthChangedEvent?.Invoke(Health);

    public void DamagePlayer(int damage)
    {
        OnHealthChangedEvent?.Invoke(Mathf.Max(Health - damage, 0));
        RPC_DamagePlayer(damage);
        if (Health - damage <= 0)
            TriggerDeath();
    }

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
            TriggerDeath();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_DamagePlayer(int damage)
    {
        Health = Mathf.Max(Health - damage, 0);
        if (Health <= 0)
            TriggerDeath();
    }
    private void TriggerDeath() => deathState.DeathSequence();
}
