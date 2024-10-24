﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Numerics;
using WorkLifeBalance.Interfaces;
using WorkLifeBalance.Services;
using WorkLifeBalance.Services.Feature;

namespace WorkLifeBalance.ViewModels;

public partial class BackgroundProcessesViewPageVM : SecondWindowPageVMBase
{
    public ObservableCollection<string> DetectedWindows { get; set; } = [];
    public ObservableCollection<string> SelectedWindows { get; set; } = [];

    [ObservableProperty]
    private string activeWindow = "";

    private readonly DataStorageFeature dataStorageFeature;
    private readonly LowLevelHandler lowLevelHandler;
    private readonly ActivityTrackerFeature activityTrackerFeature;
    private readonly ISecondWindowService secondWindowService;
    public BackgroundProcessesViewPageVM(
        DataStorageFeature dataStorageFeature,
        LowLevelHandler lowLevelHandler,
        ActivityTrackerFeature activityTrackerFeature,
        ISecondWindowService secondWindowService)
    {
        RequiredWindowSize = new Vector2(700, 570);
        WindowPageName = "Customize Work Apps";
        this.dataStorageFeature = dataStorageFeature;
        this.lowLevelHandler = lowLevelHandler;
        this.activityTrackerFeature = activityTrackerFeature;
        this.secondWindowService = secondWindowService;
    }

    private void UpdateActiveWindowUi(string newwindow) => ActiveWindow = newwindow;

    private void InitializeProcessNames()
    {
        SelectedWindows = new ObservableCollection<string>(dataStorageFeature.AutoChangeData.WorkingStateWindows);
        List<string> allProcesses = lowLevelHandler.GetBackgroundApplicationsName();
        DetectedWindows = new ObservableCollection<string>(allProcesses.Except(SelectedWindows));
    }

    public override Task OnPageOppeningAsync(object? args = null)
    {
        activityTrackerFeature.OnWindowChange += UpdateActiveWindowUi;
        InitializeProcessNames();
        return Task.CompletedTask;
    }

    public override async Task OnPageClosingAsync()
    {
        activityTrackerFeature.OnWindowChange -= UpdateActiveWindowUi;
        dataStorageFeature.AutoChangeData.WorkingStateWindows = [.. SelectedWindows];
        await dataStorageFeature.SaveData();
    }

    [RelayCommand]
    private void ReturnToPreviousPage() => secondWindowService.OpenWindowWith<OptionsPageVM>();

    [RelayCommand]
    private void SelectProcess(string processName)
    {
        DetectedWindows.Remove(processName);
        SelectedWindows.Add(processName);
    }

    [RelayCommand]
    private void DeselectProcess(string processName)
    {
        SelectedWindows.Remove(processName);
        DetectedWindows.Add(processName);
    }
}
