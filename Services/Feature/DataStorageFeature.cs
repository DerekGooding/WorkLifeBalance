﻿using WorkLifeBalance.Models;

namespace WorkLifeBalance.Services.Feature;

public class DataStorageFeature(DataBaseHandler dataBaseHandler) : FeatureBase
{
    public bool IsAppSaving { get; private set; }
    public bool IsAppLoading { get; private set; }

    public bool IsClosingApp = false;
    public bool IsAppReady = false;

    public DayData TodayData = new();
    public AppSettingsData Settings = new();
    public AutoStateChangeData AutoChangeData = new();

    public event Action? OnLoading;
    public event Action? OnLoaded;
    public event Action? OnSaving;
    public event Action? OnSaved;
    private readonly DataBaseHandler dataBaseHandler = dataBaseHandler;

    public async Task SaveData()
    {
        if (IsAppSaving) return;

        Log.Information("Saving...");

        IsAppSaving = true;

        OnSaving?.Invoke();

        await dataBaseHandler.WriteDay(TodayData);
        await dataBaseHandler.WriteSettings(Settings);
        await dataBaseHandler.WriteAutoSateData(AutoChangeData);

        OnSaved?.Invoke();

        IsAppSaving = false;

        Log.Information("Save Complete!");
    }

    public async Task LoadData()
    {
        if (IsAppLoading) return;

        IsAppLoading = true;

        OnLoading?.Invoke();

        Log.Information("Loading Day");
        TodayData = await dataBaseHandler.ReadDay(TodayData.DateC.ToString("MMddyyyy"));
        Log.Information("Loading Settings");
        Settings = await dataBaseHandler.ReadSettings();
        Log.Information("Loading Activities");
        AutoChangeData = await dataBaseHandler.ReadAutoStateData(TodayData.DateC.ToString("MMddyyyy"));

        OnLoaded?.Invoke();

        IsAppLoading = false;
        Log.Information("Load Complete!");
    }

    protected override Func<Task> ReturnFeatureMethod() => TriggerSaveData;

    private async Task TriggerSaveData()
    {
        if (IsFeatureRuning) return;

        IsFeatureRuning = true;

        try
        {
            await Task.Delay(Settings.SaveInterval * 60000, CancelTokenS.Token);
            await SaveData();
        }
        catch (TaskCanceledException taskCancel)
        {
            Log.Information($"DataStorage: {taskCancel.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataStorage");
        }
        finally
        {
            IsFeatureRuning = false;
        }
    }
}
