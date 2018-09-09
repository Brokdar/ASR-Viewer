using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using Prism.Commands;
using Prism.Mvvm;

namespace Viewer.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public ICommand ChangeThemeCommand { get; }
        public ICommand ChangeAccentColorCommand { get; }

        public SettingsViewModel()
        {
            ChangeThemeCommand = new DelegateCommand<string>(OnChangeThemeClicked);
            ChangeAccentColorCommand = new DelegateCommand<string>(OnChangeAccentColorClicked);
        }

        private static void OnChangeThemeClicked(string theme)
        {
            ThemeManager.ChangeAppTheme(Application.Current, theme);
        }

        private static void OnChangeAccentColorClicked(string accent)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accent), theme.Item1);
        }
    }
}