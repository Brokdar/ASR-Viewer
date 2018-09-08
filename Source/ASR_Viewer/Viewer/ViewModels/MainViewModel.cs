using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Regions;
using Reader;
using Shared;

namespace Viewer.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IRegistrationService _registrationService;
        private readonly AsrReader _reader = new AsrReader();

        private string _title = "ASR Viewer";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ObservableCollection<IPlugin> Plugins { get; } = new ObservableCollection<IPlugin>();


        public MainViewModel(RegistrationService registrationService, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _registrationService = registrationService;

            registrationService.NewRegistration += UpdatePluginList;
        }

        private void UpdatePluginList(object sender, NewRegistrationArgs args)
        {
            Plugins.Add(args.Plugin);
        }
    }
}