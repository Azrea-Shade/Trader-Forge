using System.Windows;

namespace AzreaCompanion
{
    public partial class MainWindow : Window
    {
        public MainWindow(Presentation.MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
