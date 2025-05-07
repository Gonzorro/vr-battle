using UnityEngine;
using System.Collections.Generic;

public enum ParticleType { LauncherExplosion, Hit, Sparkle, Smoke }

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private List<ParticleEntry> particlePrefabs;

    private static ParticleManager instance;
    public static ParticleManager Instance => instance;

    private Dictionary<ParticleType, Queue<PooledParticle>> pools = new();

    [System.Serializable]
    private class ParticleEntry
    {
        public ParticleType type;
        public PooledParticle prefab;
        public int poolSize = 10;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in particlePrefabs)
        {
            var queue = new Queue<PooledParticle>();
            for (int i = 0; i < entry.poolSize; i++)
                queue.Enqueue(CreateParticleInstance(entry.prefab, entry.type));
            pools[entry.type] = queue;
        }
    }

    private PooledParticle CreateParticleInstance(PooledParticle prefab, ParticleType type)
    {
        var instance = Instantiate(prefab, transform);
        instance.SetType(type);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public void Play(ParticleType type, Vector3 position, Quaternion rotation = default)
    {
        if (!pools.TryGetValue(type, out var pool))
        {
            pool = new Queue<PooledParticle>();
            pools[type] = pool;
        }

        var instance = pool.Count > 0 ? pool.Dequeue() : CreateParticleInstance(GetPrefabByType(type), type);

        if (instance == null)
        {
            Debug.LogWarning($"No prefab found for type: {type}");
            return;
        }

        instance.transform.SetPositionAndRotation(position, rotation == default ? Quaternion.identity : rotation);
        instance.gameObject.SetActive(true);
    }

    public void ReturnToPool(ParticleType type, PooledParticle particle)
    {
        if (!pools.ContainsKey(type))
            pools[type] = new Queue<PooledParticle>();

        pools[type].Enqueue(particle);
    }

    private PooledParticle GetPrefabByType(ParticleType type)
    {
        foreach (var entry in particlePrefabs)
            if (entry.type == type)
                return entry.prefab;
        return null;
    }
}
