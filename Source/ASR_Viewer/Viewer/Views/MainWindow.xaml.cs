using Prism.Regions;

namespace Viewer.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();

            RegionManager.SetRegionName(MainContent, "ModuleRegion");
            RegionManager.SetRegionManager(MainContent, regionManager);
        }
    }
}
