using CommunityToolkit.Mvvm.ComponentModel;
using System.Numerics;

namespace WorkLifeBalance.ViewModels;

public abstract partial class SecondWindowPageVMBase : ViewModelBase
{
    [ObservableProperty]
    public Vector2 requiredWindowSize = new Vector2(250, 300);
    
    [ObservableProperty]
    public string windowPageName = "Page";

    public virtual Task OnPageClosingAsync() => Task.CompletedTask;

    public virtual Task OnPageOppeningAsync(object? args = null) => Task.CompletedTask;
}
