using System;
using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class PoolObjectProvider : Fusion.Behaviour, INetworkObjectProvider
{
    [SerializeField] private bool delayIfSceneManagerIsBusy = true;
    [SerializeField] private int maxPoolCount = 0;

    private readonly Dictionary<NetworkPrefabId, Queue<NetworkObject>> pool = new();

    private NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab, NetworkPrefabId prefabId)
    {
        if (pool.TryGetValue(prefabId, out var freeQ) && freeQ.Count > 0)
            return DequeueAndActivate(freeQ);

        if (!pool.ContainsKey(prefabId))
            pool[prefabId] = new Queue<NetworkObject>();

        return Instantiate(prefab);
    }

    private NetworkObject DequeueAndActivate(Queue<NetworkObject> freeQ)
    {
        var instance = freeQ.Dequeue();
        instance.gameObject.SetActive(true);
        return instance;
    }

    private void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
    {
        if (!pool.TryGetValue(prefabId, out var freeQ))
        {
            Destroy(instance.gameObject);
            return;
        }

        if (maxPoolCount > 0 && freeQ.Count >= maxPoolCount)
        {
            Destroy(instance.gameObject);
            return;
        }

        instance.gameObject.SetActive(false);
        freeQ.Enqueue(instance);
    }

    public NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
    {
        if (delayIfSceneManagerIsBusy && runner.SceneManager.IsBusy)
        { instance = null; return NetworkObjectAcquireResult.Retry; }

        NetworkObject prefab;
        try { prefab = runner.Prefabs.Load(context.PrefabId, isSynchronous: context.IsSynchronous); }
        catch (Exception ex) { Debug.LogError($"[Pool] Failed to load prefab: {ex}"); instance = null; return NetworkObjectAcquireResult.Retry; }

        if (prefab == null)
        { instance = null; return NetworkObjectAcquireResult.Retry; }

        instance = InstantiatePrefab(runner, prefab, context.PrefabId);
        if (context.DontDestroyOnLoad) runner.MakeDontDestroyOnLoad(instance.gameObject);
        else runner.MoveToRunnerScene(instance.gameObject);

        runner.Prefabs.AddInstance(context.PrefabId);
        return NetworkObjectAcquireResult.Success;
    }

    public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
    {
        var instance = context.Object;
        var typeId = context.TypeId;

        if (!context.IsBeingDestroyed)
            if (typeId.IsPrefab)
                DestroyPrefabInstance(runner, typeId.AsPrefabId, instance);
            else
                Destroy(instance.gameObject);

        if (typeId.IsPrefab)
            runner.Prefabs.RemoveInstance(typeId.AsPrefabId);
    }

    public NetworkPrefabId GetPrefabId(NetworkRunner runner, NetworkObjectGuid prefabGuid) =>
        runner.Config.PrefabTable.GetId(prefabGuid);

    public void SetMaxPoolCount(int count) => maxPoolCount = count;
}
