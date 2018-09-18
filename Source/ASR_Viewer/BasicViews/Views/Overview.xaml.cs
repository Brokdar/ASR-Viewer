using System.Windows;
using System.Windows.Documents;
using BasicViews.ViewModels;

namespace BasicViews.Views
{
    /// <summary>
    /// Interaktionslogik für Overview.xaml
    /// </summary>
    public partial class Overview
    {
        public Overview()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is OverviewViewModel viewmodel)) return;

            var referenceToElement = ((Hyperlink) e.Source).DataContext as XElementViewModel;
            viewmodel.NavigateToReference(referenceToElement?.Value);
        }
    }
}
