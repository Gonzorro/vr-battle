using Fusion;
using UnityEngine;
using System.Collections;

public class SimulationSync : NetworkBehaviour
{
    public static SimulationSync Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// StateAuthority schedules a method to be called back on all clients at a specific time.
    /// Method must be Public!
    /// </summary>
    /// <param name="methodName">Name of the method to call.</param>
    /// <param name="targetBehaviour">The NetworkBehaviour on which the method exists.</param>
    /// <param name="delay">The delay (in seconds) before the method is called.</param>
    public void ScheduleCallback(string methodName, NetworkBehaviour targetBehaviour, float delay)
    {
        if (!HasStateAuthority) return;

        float targetTime = Runner.SimulationTime + delay;
        RPC_ScheduleCallback(methodName, targetBehaviour, targetTime);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ScheduleCallback(string methodName, NetworkBehaviour targetBehaviour, float targetTime)
        => StartCoroutine(WaitAndInvoke(methodName, targetBehaviour, targetTime));

    private IEnumerator WaitAndInvoke(string methodName, NetworkBehaviour targetBehaviour, float targetTime)
    {
        var method = targetBehaviour.GetType().GetMethod(methodName);

        while (Runner.SimulationTime < targetTime)
            yield return null;

        Debug.LogError("Scheduled Callback time: " + Runner.SimulationTime);
        method?.Invoke(targetBehaviour, null);
    }

}
