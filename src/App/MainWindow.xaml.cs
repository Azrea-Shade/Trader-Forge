using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace App
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
