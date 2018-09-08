using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using Shared.ASR;

namespace BasicViews.ViewModels
{
    public class OverviewViewModel : BindableBase
    {
        public Document Document { get; set; }

        public IEnumerable<string> Packages => Document.Packages.Select(p => p.Name);
        public string SelectedPackage { get; set; }

        public OverviewViewModel()
        {

        }
    }
}