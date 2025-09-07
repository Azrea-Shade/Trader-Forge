using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace AzreaCompanion
{
    public partial class MainWindow : Window
    {
        public MainWindow(Presentation.MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var about = AzreaCompanion.App.Services.GetService<AboutWindow>();
            about?.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
    }
}
