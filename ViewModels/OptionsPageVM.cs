﻿using CommunityToolkit.Mvvm.Input;
using System.Numerics;
using WorkLifeBalance.Interfaces;

namespace WorkLifeBalance.ViewModels;

public partial class OptionsPageVM : SecondWindowPageVMBase
{
    private ISecondWindowService secondWindowService;
    public OptionsPageVM(ISecondWindowService secondWindowService)
    {
        this.secondWindowService = secondWindowService;
        RequiredWindowSize = new Vector2(250, 320);
        WindowPageName = "Options";
    }

    [RelayCommand]
    private void OpenSettings() => secondWindowService.OpenWindowWith<SettingsPageVM>();

    [RelayCommand]
    private void ConfigureAutoDetect() => secondWindowService.OpenWindowWith<BackgroundProcessesViewPageVM>();
}
