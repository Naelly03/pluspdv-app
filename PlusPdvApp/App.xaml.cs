using PlusPdvApp.Services;
using PlusPdvApp.ViewModels;
using System.Windows;

namespace PlusPdvApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var apiService = new ApiService();

            var viewModel = new MainWindowViewModel(apiService);

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
        }
    }
}