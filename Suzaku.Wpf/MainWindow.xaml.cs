using Microsoft.Web.WebView2.Core;
using NHotkey;
using NHotkey.Wpf;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Suzaku.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.Manual;
            Width = 400;
            Height = SystemParameters.PrimaryScreenHeight - 60;
            Left = SystemParameters.PrimaryScreenWidth - Width;
            Top = 10;
            Topmost = true;
            WindowStyle = WindowStyle.None;

            HotkeyManager.Current.AddOrReplace(
                "ShowOrHide",
                Key.Oem2,
                ModifierKeys.Windows,
                OnGlobalHotkey
            );

            webView2.NavigationStarting += (s, e) =>
                webView2.CoreWebView2.Profile.IsGeneralAutofillEnabled = false;
        }

        private void OnLocalHotkey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ToggleVisibility();
                e.Handled = true;
            }
        }

        private void OnGlobalHotkey(object? sender, HotkeyEventArgs e)
        {
            ToggleVisibility();
            e.Handled = true;
        }

        private void ToggleVisibility()
        {
            this.Visibility =
                this.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            if (this.Visibility == Visibility.Visible)
            {
                Focus();
            }
        }
    }
}
