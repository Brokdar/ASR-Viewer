using System.Collections.Generic;
using System.Xml.Linq;
using Prism.Mvvm;
using Prism.Regions;
using Shared.ASR;

namespace BasicViews.ViewModels
{
    public class OverviewViewModel : BindableBase, INavigationAware
    {
        private Document _document;

        private XElement _arxml;
        public XElement Arxml
        {
            get => _arxml;
            set => SetProperty(ref _arxml, value);
        }

        private int _packageCount;
        public int PackageCount
        {
            get => _packageCount;
            set => SetProperty(ref _packageCount, value);
        }

        private IList<Package> _packages;
        public IList<Package> Packages
        {
            get => _packages;
            set => SetProperty(ref _packages, value);
        }

        private Package _selectedPackage;
        public Package SelectedPackage
        {
            get => _selectedPackage;
            set => SetProperty(ref _selectedPackage, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!(navigationContext.Parameters["Document"] is Document doc)) return;

            _document = doc;
            Packages = new List<Package>(doc.Packages);
            PackageCount = _packages.Count;
            Arxml = _document.Root;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}