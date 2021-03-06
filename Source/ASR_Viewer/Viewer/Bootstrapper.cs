﻿using System.Windows;
using Prism.Modularity;
using Prism.Unity;
using Viewer.Views;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Shared;
using Viewer.ViewModels;

namespace Viewer
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow?.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IRegistrationService, RegistrationService>(new ContainerControlledLifetimeManager());
            Container.RegisterTypeForNavigation<Settings>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainViewModel>();
            ViewModelLocationProvider.Register<Settings, SettingsViewModel>();
        }
        
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog
            {
                ModulePath = @".\Modules"
            };
        }
    }
}