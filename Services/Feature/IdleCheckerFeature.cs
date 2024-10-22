using System.Numerics;
using WorkLifeBalance.Interfaces;

namespace WorkLifeBalance.Services.Feature;

public class IdleCheckerFeature(DataStorageFeature dataStorageFeature, LowLevelHandler lowLevelHandler, AppStateHandler appStateHandler, IFeaturesServices featuresServices) : FeatureBase
{
    private Vector2 _oldmousePosition = new(-1, -1);
    private readonly AppStateHandler appStateHandler = appStateHandler;
    private readonly DataStorageFeature dataStorageFeature = dataStorageFeature;
    private readonly LowLevelHandler lowLevelHandler = lowLevelHandler;
    private readonly IFeaturesServices featuresServices = featuresServices;

    //private readonly int MinuteMiliseconds = 60000;
    //private readonly int IdleDelay = 3000;
    //private readonly int RestingDelay = 600000;

    protected override Func<Task> ReturnFeatureMethod() => TriggerCheckIdle;

    private async Task TriggerCheckIdle()
    {
        if (IsFeatureRuning) return;

        try
        {
            IsFeatureRuning = true;
            int delay = appStateHandler.AppTimerState == AppState.Idle
                ? 2000
                : dataStorageFeature.Settings.AutoDetectIdleInterval * 60000 / 2;
            await Task.Delay(delay, CancelTokenS.Token);
            CheckIdle();
        }
        catch (TaskCanceledException taskCancel)
        {
            Log.Information($"Idle Checker: {taskCancel.Message}");
        }
        catch(Exception ex)
        {
            Log.Error(ex,"Idle Checker");
        }
        finally
        {
            IsFeatureRuning = false;
        }
    }

    private void CheckIdle()
    {
        Vector2 newpos = Vector2.Zero;

        try
        {
            newpos = lowLevelHandler.GetMousePos();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }

        if (_oldmousePosition == new Vector2(-1, -1))
        {
            _oldmousePosition = newpos;
            return;
        }

        if (newpos == _oldmousePosition)
        {
            featuresServices.RemoveFeature<StateCheckerFeature>();
            appStateHandler.SetAppState(AppState.Idle);
        }
        else
        {
            featuresServices.AddFeature<StateCheckerFeature>();
        }

        _oldmousePosition = newpos;
    }
}
