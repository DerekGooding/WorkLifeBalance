﻿using CommunityToolkit.Mvvm.ComponentModel;
using IWshRuntimeLibrary;
using System.Numerics;
using WorkLifeBalance.Interfaces;
using WorkLifeBalance.Services.Feature;
using File = System.IO.File;
using Path = System.IO.Path;

namespace WorkLifeBalance.ViewModels;

public partial class SettingsPageVM : SecondWindowPageVMBase
{
    [ObservableProperty]
    private string version = "";

    [ObservableProperty]
    private int autoSaveInterval = 5;

    [ObservableProperty]
    private int autoDetectInterval = 1;

    [ObservableProperty]
    private int autoDetectIdleInterval = 1;

    [ObservableProperty]
    private bool startWithWin = false;

    [ObservableProperty]
    private int[]? numbers;

    private readonly DataStorageFeature dataStorageFeature;
    private readonly IFeaturesServices featuresServices;
    public SettingsPageVM(DataStorageFeature dataStorageFeature, IFeaturesServices featuresServices)
    {
        this.featuresServices = featuresServices;
        this.dataStorageFeature = dataStorageFeature;
        RequiredWindowSize = new Vector2(250, 320);
        WindowPageName = "Settings";

        InitializeData();
    }

    private void InitializeData()
    {
        Version = $"Version: {dataStorageFeature.Settings.Version}";

        AutoSaveInterval = dataStorageFeature.Settings.SaveInterval;

        AutoDetectInterval = dataStorageFeature.Settings.AutoDetectInterval;

        AutoDetectIdleInterval = dataStorageFeature.Settings.AutoDetectIdleInterval;

        StartWithWin = dataStorageFeature.Settings.StartWithWindowsC;

        List<int> numbersTemp = [];
        for(int x = 1; x <= 300; x++)
        {
            numbersTemp.Add(x);
        }
        Numbers = [.. numbersTemp];
    }

    public override async Task OnPageClosingAsync()
    {
        dataStorageFeature.Settings.SaveInterval = AutoSaveInterval;

        dataStorageFeature.Settings.AutoDetectInterval = AutoDetectInterval;

        dataStorageFeature.Settings.AutoDetectIdleInterval = AutoDetectIdleInterval;

        dataStorageFeature.Settings.StartWithWindowsC = StartWithWin;

        await dataStorageFeature.SaveData();

        ApplyStartToWindows();
        dataStorageFeature.Settings.OnSettingsChanged.Invoke();
    }

    private void ApplyStartToWindows()
    {
        FileInfo startupFolderPath = new(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            $"{dataStorageFeature.Settings.Version}.lnk"));

        if (dataStorageFeature.Settings.StartWithWindowsC)
        {
            CreateShortcut(startupFolderPath);
        }
        else
        {
            DeleteShortcut(startupFolderPath);
        }
    }

    private void CreateShortcut(FileInfo startup)
    {
        if (!startup.Exists)
        {
            WshShell shell = new();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(startup.FullName);
            shortcut.TargetPath = dataStorageFeature.Settings.AppDirectory;
            shortcut.WorkingDirectory = dataStorageFeature.Settings.AppDirectory;
            shortcut.Save();
        }
    }

    private static void DeleteShortcut(FileInfo startup)
    {
        if (startup.Exists)
        {
            startup.Delete();
        }
    }
}
