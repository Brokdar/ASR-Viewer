using System.Collections.Generic;
using System.Collections.ObjectModel;
using BasicViews.Services;
using Prism.Mvvm;
using Prism.Regions;
using Shared.ASR;

namespace BasicViews.ViewModels
{
    public class OverviewViewModel : BindableBase, INavigationAware
    {
        private Document _document;

        private ReadOnlyCollection<XElementViewModel> _root;
        public ReadOnlyCollection<XElementViewModel> Root
        {
            get => _root;
            set => SetProperty(ref _root, value);
        }

        private object[] _selectPathItem;
        public object[] SelectPathItem
        {
            get => _selectPathItem;
            set
            {
                if (_selectPathItem != value)
                    SetProperty(ref _selectPathItem, value);
            }
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
            set
            {
                SetProperty(ref _selectedPackage, value);
                SelectPackageElement(value.Uuid);
            }
        }

        private void SelectPackageElement(string uuid)
        {
            var packages = XSearchService.FindElementByName(Root[0], "AR-PACKAGES");
            var package = XSearchService.FindElementByAttribute(packages, "UUID", uuid);

            if (package == null)
                return;

            var elements = Root[0].Elements;
            elements[0].IsExpanded = false;

            foreach (var element in elements[1].Elements)
            {
                element.IsExpanded = false;
            }

            SelectPathItem = XSearchService.GetPathFromRootTo(package);
            package.IsExpanded = true;
            package.IsSelected = true;
        }

        #region INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!(navigationContext.Parameters["Document"] is Document doc)) return;

            _document = doc;
            Packages = new List<Package>(doc.Packages);
            PackageCount = Packages.Count;
            Root = new ReadOnlyCollection<XElementViewModel>(
                new[]
                {
                    new XElementViewModel(_document.Root)
                });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}