﻿using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Diagnostics;
using System.Windows;
using WorkLifeBalance.Interfaces;
using WorkLifeBalance.Services;
using WorkLifeBalance.Services.Feature;
using WorkLifeBalance.ViewModels;

namespace WorkLifeBalance
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool Debug = true;
        private readonly ServiceProvider _servicesProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

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
            services.AddSingleton<DataBaseHandler>();
            services.AddSingleton<LowLevelHandler>();
            services.AddSingleton<AppTimer>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISecondWindowService, SecondWindowService>();
            services.AddSingleton<IFeaturesServices, FeaturesService>();

            //factory method for ViewModelBase.
            services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));
            //factory method for Features.
            services.AddSingleton<Func<Type, FeatureBase>>(serviceProvider => featureBase => (FeatureBase)serviceProvider.GetRequiredService(featureBase));

            services.AddSingleton<BackgroundProcessesViewPageVM>();
            services.AddSingleton<MainMenuVM>();
            services.AddSingleton<OptionsPageVM>();
            services.AddSingleton<SecondWindowVM>();
            services.AddSingleton<SettingsPageVM>();
            services.AddSingleton<ViewDataPageVM>();
            services.AddSingleton<ViewDayDetailsPageVM>();
            services.AddSingleton<ViewDaysPageVM>();

            _servicesProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LowLevelHandler lowHandler = _servicesProvider.GetRequiredService<LowLevelHandler>();
            DataStorageFeature dataStorageFeature = _servicesProvider.GetRequiredService<DataStorageFeature>();

            //move show a popup and then if the user pressses ok, restart, if not, close app
            //if (!lowHandler.IsRunningAsAdmin())
            //{
            //    RestartApplicationWithAdmin();
            //    return;
            //}

            //use a json config to get the debug bool value
            if (Debug)
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

            dataStorageFeature.OnLoaded += InitializeApp;

            _ = dataStorageFeature.LoadData();
        }

        //initialize app called when data was loaded
        private void InitializeApp()
        {
            try
            {
                DataStorageFeature dataStorageFeature = _servicesProvider.GetRequiredService<DataStorageFeature>();
                AppTimer appTimer = _servicesProvider.GetRequiredService<AppTimer>();
                //set app ready so timers can start
                dataStorageFeature.IsAppReady = true;

                IFeaturesServices featuresService = _servicesProvider.GetRequiredService<IFeaturesServices>();
                featuresService.AddFeature<DataStorageFeature>();
                featuresService.AddFeature<TimeTrackerFeature>();
                featuresService.AddFeature<ActivityTrackerFeature>();

                //check settings to see if you need to add some features
                if (dataStorageFeature.Settings.AutoDetectWorkingC)
                {
                    featuresService.AddFeature<StateCheckerFeature>();
                }

                //starts the main timer
                appTimer.StartTick();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            //SetAppState(AppState.Resting);

            var mainWindow = _servicesProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Log.Information("------------------App Initialized------------------");
        }

        private void RestartApplicationWithAdmin()
        {
            var DataStorageFeature = _servicesProvider.GetRequiredService<DataStorageFeature>();
            var psi = new ProcessStartInfo
            {
                FileName = DataStorageFeature.AppExePath,
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(psi);
            Current.Shutdown();
        }
    }
}
