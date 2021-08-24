using System.Windows;
using CachedPathSuggestBox.Demo.ViewModel;

namespace CachedPathSuggestBox.Demo
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppViewModel();
        }
    }
}