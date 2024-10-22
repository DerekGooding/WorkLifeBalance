namespace WorkLifeBalance.Services.Feature;

public class TimeTrackerFeature(DataStorageFeature dataStorageFeature, AppStateHandler appStateHandler) : FeatureBase
{
    public delegate void SpentTimeEvent();
    public event SpentTimeEvent? OnSpentTimeChange;

    private readonly TimeSpan OneSec = new (0, 0, 1);
    private readonly AppStateHandler appStateHandler = appStateHandler;
    private readonly DataStorageFeature dataStorageFeature = dataStorageFeature;

    protected override Func<Task> ReturnFeatureMethod() => TriggerUpdateSpentTime;

    private Task TriggerUpdateSpentTime()
    {
        switch (appStateHandler.AppTimerState)
        {
            case AppState.Working:
                dataStorageFeature.TodayData.WorkedAmmountC = dataStorageFeature.TodayData.WorkedAmmountC.Add(OneSec);
                break;

            case AppState.Resting:
                dataStorageFeature.TodayData.RestedAmmountC = dataStorageFeature.TodayData.RestedAmmountC.Add(OneSec);
                break;

            case AppState.Idle:
                dataStorageFeature.TodayData.IdleAmmountC = dataStorageFeature.TodayData.IdleAmmountC.Add(OneSec);
                break;
        }
        OnSpentTimeChange?.Invoke();
        return Task.CompletedTask;
    }
}

public enum AppState
{
    Working,
    Resting,
    Idle
}
