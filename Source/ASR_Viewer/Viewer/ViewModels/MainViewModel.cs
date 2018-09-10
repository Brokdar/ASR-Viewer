using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Reader;
using Shared;
using Shared.ASR;

namespace Viewer.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private Document _document;

        private const string BaseTitle = "ASR Viewer";
        private string _title = BaseTitle;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private readonly List<IPlugin> _plugins = new List<IPlugin>();

        private IReadOnlyList<IPlugin> _readOnlyPlugins;
        public IReadOnlyList<IPlugin> Plugins
        {
            get => _readOnlyPlugins;
            set => SetProperty(ref _readOnlyPlugins, value);
        }

        public ICommand OpenCommand { get; }
        public ICommand OptionItemClickedCommand { get; }
        public ICommand NavigationItemClickedCommand { get; }

        public MainViewModel(RegistrationService registrationService, IRegionManager regionManager)
        {
            _regionManager = regionManager;

            registrationService.NewRegistration += UpdatePluginList;

            OpenCommand = new DelegateCommand(OnOpenClicked);
            OptionItemClickedCommand = new DelegateCommand(OnOptionItemClicked);
            NavigationItemClickedCommand = new DelegateCommand(OnNavigationItemClicked);
        }

        private void UpdatePluginList(object sender, NewRegistrationArgs args)
        {
            _plugins.Add(args.Plugin);
        }

        private void OnOpenClicked()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "ARXML files (*.arxml)|*.arxml"
            };

            if (dialog.ShowDialog() != true)
                return;

            _document = new AsrReader().Read(dialog.FileName);
            Title = BaseTitle + " | " + _document.Info.Path;
            Plugins = _plugins;
        }

        private void OnOptionItemClicked()
        {
            _regionManager.RequestNavigate("ModuleRegion", "Settings");
        }

        private void OnNavigationItemClicked()
        {
            var parameter = new NavigationParameters {{"Document", _document}};
            _regionManager.RequestNavigate("ModuleRegion", "Overview", parameter);
        }
    }
}