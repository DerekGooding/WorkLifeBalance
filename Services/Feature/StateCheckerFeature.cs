namespace WorkLifeBalance.Services.Feature;

public class StateCheckerFeature(DataStorageFeature dataStorageFeature, ActivityTrackerFeature activityTrackerFeature, AppStateHandler appStateHandler) : FeatureBase
{
    public bool IsFocusingOnWorkingWindow = false;
    private readonly DataStorageFeature dataStorageFeature = dataStorageFeature;
    private readonly ActivityTrackerFeature activityTrackerFeature = activityTrackerFeature;
    private readonly AppStateHandler appStateHandler = appStateHandler;

    protected override Func<Task> ReturnFeatureMethod() => TriggerWorkDetect;

    private async Task TriggerWorkDetect()
    {
        if (IsFeatureRuning) return;

        try
        {
            IsFeatureRuning = true;
            await Task.Delay(dataStorageFeature.Settings.AutoDetectInterval * 1000, CancelTokenS.Token);
            CheckStateChange();
        }
        catch (TaskCanceledException taskCancel)
        {
            Log.Information($"State Checker: {taskCancel.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "State Checker");
        }
        finally
        {
            IsFeatureRuning = false;
        }
    }

    private void CheckStateChange()
    {
        if (string.IsNullOrEmpty(activityTrackerFeature.ActiveWindow)) return;

        IsFocusingOnWorkingWindow = dataStorageFeature.AutoChangeData.WorkingStateWindows.Contains(activityTrackerFeature.ActiveWindow);

        switch (appStateHandler.AppTimerState)
        {
            case AppState.Working:
                if (!IsFocusingOnWorkingWindow)
                {
                    appStateHandler.SetAppState(AppState.Resting);
                }
                break;

            case AppState.Resting:
                if (IsFocusingOnWorkingWindow)
                {
                    appStateHandler.SetAppState(AppState.Working);
                }
                break;
            case AppState.Idle:
                if (IsFocusingOnWorkingWindow)
                {
                    appStateHandler.SetAppState(AppState.Working);
                }
                else
                {
                    appStateHandler.SetAppState(AppState.Resting);
                }
                break;
        }
    }
}
