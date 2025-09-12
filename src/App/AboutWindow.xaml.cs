using Services;
using System.Windows;

namespace AzreaCompanion
{
    public partial class AboutWindow : Window
    {
        public AboutWindow(Presentation.AboutViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void OnClose(object sender, RoutedEventArgs e) => Close();
    }
}
