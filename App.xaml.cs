﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WorkLifeBalance.Services.Feature;
using WorkLifeBalance.Interfaces;
using WorkLifeBalance.ViewModels;
using WorkLifeBalance.Services;
using System.Diagnostics;

namespace WorkLifeBalance;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App() => _host = Host.CreateDefaultBuilder()
               .ConfigureAppConfiguration((context, config) =>
                   config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                         .AddEnvironmentVariables())
               .ConfigureServices((context, services) =>
               {
                   // Pass the configuration object
                   services.AddSingleton(context.Configuration);

                   // Register services and view models here
                   ConfigureServices(services);
               })
               .Build();

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainMenuVM>()
        });
        services.AddSingleton(provider => new SecondWindow
        {
            DataContext = provider.GetService<SecondWindowVM>()
        });
        services.AddSingleton<DataStorageFeature>();
        services.AddSingleton<ActivityTrackerFeature>();
        services.AddSingleton<IdleCheckerFeature>();
        services.AddSingleton<StateCheckerFeature>();
        services.AddSingleton<TimeTrackerFeature>();
        services.AddSingleton<SqlDataAccess>();
        services.AddSingleton<DataBaseHandler>();
        services.AddSingleton<LowLevelHandler>();
        services.AddSingleton<AppStateHandler>();
        services.AddSingleton<SqlLiteDatabaseIntegrity>();
        services.AddSingleton<AppTimer>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISecondWindowService, SecondWindowService>();
        services.AddSingleton<IFeaturesServices, FeaturesService>();

        //factory method for ViewModelBase.
        services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType =>
            (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));
        //factory method for Features.
        services.AddSingleton<Func<Type, FeatureBase>>(serviceProvider => featureBase =>
            (FeatureBase)serviceProvider.GetRequiredService(featureBase));

        services.AddSingleton<BackgroundProcessesViewPageVM>();
        services.AddSingleton<MainMenuVM>();
        services.AddSingleton<OptionsPageVM>();
        services.AddSingleton<SecondWindowVM>();
        services.AddSingleton<SettingsPageVM>();
        services.AddSingleton<ViewDataPageVM>();
        services.AddSingleton<LoadingPageVM>();
        services.AddSingleton<ViewDayDetailsPageVM>();
        services.AddSingleton<ViewDaysPageVM>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        LowLevelHandler lowHandler = _host.Services.GetRequiredService<LowLevelHandler>();

        //move show a popup and then if the user pressses ok, restart, if not, close app
        //if (!lowHandler.IsRunningAsAdmin())
        //{
        //    RestartApplicationWithAdmin();
        //    return;
        //}

        bool isDebug = _host.Services.GetRequiredService<IConfiguration>().GetValue<bool>("Debug");
        if (isDebug)
        {
            lowHandler.EnableConsole();
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }
        _ = InitializeApp();
    }

    private async Task InitializeApp()
    {
        DataStorageFeature dataStorageFeature = _host.Services.GetRequiredService<DataStorageFeature>();

        SqlLiteDatabaseIntegrity sqlLiteDatabaseIntegrity = _host.Services.GetRequiredService<SqlLiteDatabaseIntegrity>();

        await sqlLiteDatabaseIntegrity.CheckDatabaseIntegrity();

        await dataStorageFeature.LoadData();

        AppTimer appTimer = _host.Services.GetRequiredService<AppTimer>();

        //set app ready so timers can start
        dataStorageFeature.IsAppReady = true;

        IFeaturesServices featuresService = _host.Services.GetRequiredService<IFeaturesServices>();
        featuresService.AddFeature<DataStorageFeature>();
        featuresService.AddFeature<TimeTrackerFeature>();
        featuresService.AddFeature<ActivityTrackerFeature>();
        featuresService.AddFeature<IdleCheckerFeature>();
        featuresService.AddFeature<StateCheckerFeature>();

        //starts the main timer
        appTimer.StartTick();

        MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        Log.Information("------------------App Initialized------------------");
    }

    private void RestartApplicationWithAdmin()
    {
        DataStorageFeature DataStorageFeature = _host.Services.GetRequiredService<DataStorageFeature>();
        ProcessStartInfo psi = new()
        {
            FileName = DataStorageFeature.Settings.AppExePath,
            UseShellExecute = true,
            Verb = "runas"
        };

        Process.Start(psi);
        Current.Shutdown();
    }
}
