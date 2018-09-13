using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var package = FindPackageElement(uuid);

            if (package == null)
                return;

            var elements = Root[0].Elements;
            elements[0].IsExpanded = false;

            foreach (var element in elements[1].Elements)
            {
                element.IsExpanded = false;
            }

            SelectPathItem = GetPathFromRootTo(package);
            package.IsExpanded = true;
            package.IsSelected = true;
        }

        private XElementViewModel FindPackageElement(string uuid)
        {
            var package = Root[0].Element("AR-PACKAGES");
            foreach (var element in package.Elements)
            {
                var attribute = element.Attribute("UUID");
                if (attribute != null && attribute.Value == uuid)
                    return element;
            }

            return null;
        }

        private static XElementViewModel[] GetPathFromRootTo(XElementViewModel element)
        {
            var path = new List<XElementViewModel>{element};

            while (element.Parent != null)
            {
                path.Add(element.Parent);
                element = element.Parent;
            }

            path.Reverse();

            return path.ToArray();
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