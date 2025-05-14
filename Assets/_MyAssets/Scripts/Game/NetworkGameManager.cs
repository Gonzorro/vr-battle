using Fusion;
using UnityEngine;
using System;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [Header("Channels")]
    [SerializeField] private NetworkRunnerChannel networkRunnerChannel;

    [Header("Settings")]
    [SerializeField] private int startingPoints = 100;

    [Header("Runtime")]
    [SerializeField] private FlagController redFlag;
    [SerializeField] private FlagController blueFlag;

    [Networked, OnChangedRender(nameof(OnRedPointsChanged))]
    public int RedPoints { get; set; }

    [Networked, OnChangedRender(nameof(OnBluePointsChanged))]
    public int BluePoints { get; set; }

    [Networked] private TickTimer ScoreTickTimer { get; set; }

    private readonly float tickInterval = 1f;

    public static event Action<int> OnRedPointsUpdated;
    public static event Action<int> OnBluePointsUpdated;

    private void OnEnable()
    {
        networkRunnerChannel.OnPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(NetworkRunner arg1, PlayerRef arg2)
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        networkRunnerChannel.OnPlayerJoined
    }

    public override void Spawned()
    {
        OnRedPointsChanged();
        OnBluePointsChanged();

        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        if (!Runner.IsSharedModeMasterClient) return;

        RedPoints = startingPoints;
        BluePoints = startingPoints;
        ScoreTickTimer = TickTimer.CreateFromSeconds(Runner, tickInterval);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        if (ScoreTickTimer.Expired(Runner))
        {
            RedPoints -= GetPointLoss(redFlag);
            BluePoints -= GetPointLoss(blueFlag);
            ScoreTickTimer = TickTimer.CreateFromSeconds(Runner, tickInterval);
        }
    }

    private int GetPointLoss(FlagController flag)
    {
        if (flag.CurrentState == FlagState.Held)
            return flag.HolderTeam != flag.FlagTeam ? 2 : 0;
        if (flag.CurrentState == FlagState.Away)
            return 1;
        return 0;
    }

    public void OnFlagCaptured(bool redScored)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        if (redScored)
            BluePoints = Mathf.Max(BluePoints - 30, 0);
        else
            RedPoints = Mathf.Max(RedPoints - 30, 0);

        redFlag.ResetFlag();
        blueFlag.ResetFlag();
    }

    private void OnRedPointsChanged() => OnRedPointsUpdated?.Invoke(RedPoints);

    private void OnBluePointsChanged() => OnBluePointsUpdated?.Invoke(BluePoints);
}
