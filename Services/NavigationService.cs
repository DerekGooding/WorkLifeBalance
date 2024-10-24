﻿using CommunityToolkit.Mvvm.ComponentModel;
using WorkLifeBalance.Interfaces;
using WorkLifeBalance.ViewModels;

namespace WorkLifeBalance.Services;

public partial class NavigationService(Func<Type, ViewModelBase> viewModelFactory) : ObservableObject, INavigationService
{
    [ObservableProperty]
    public ViewModelBase? activeView;
    private readonly Func<Type, ViewModelBase> _viewModelFactory = viewModelFactory;

    public void NavigateTo<TViewModelbase>() where TViewModelbase : ViewModelBase
    {
        ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TViewModelbase));
        ActiveView = viewModel;
    }
}
