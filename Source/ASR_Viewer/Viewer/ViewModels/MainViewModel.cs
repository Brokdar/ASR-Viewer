using System;
using System.Collections.ObjectModel;
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
        private readonly IRegistrationService _registrationService;
        private Document _document;

        private const string BaseTitle = "ASR Viewer";
        private string _title = BaseTitle;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ObservableCollection<IPlugin> Plugins { get; } = new ObservableCollection<IPlugin>();

        public ICommand OpenCommand { get; }

        public MainViewModel(RegistrationService registrationService, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _registrationService = registrationService;

            registrationService.NewRegistration += UpdatePluginList;

            OpenCommand = new DelegateCommand(OpenNewDocument);
        }

        private void OpenNewDocument()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "ARXML files (*.arxml)|*.arxml"
            };

            if (dialog.ShowDialog() != true)
                return;

            _document = new AsrReader().Read(dialog.FileName);
            Title = BaseTitle + " | " + _document.Info.Path;
        }

        private void UpdatePluginList(object sender, NewRegistrationArgs args)
        {
            Plugins.Add(args.Plugin);
        }
    }
}