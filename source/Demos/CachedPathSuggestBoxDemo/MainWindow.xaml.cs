using System.Windows;
using CachedPathSuggestBoxDemo.Infrastructure;
using CachedPathSuggestBoxDemo.ViewModels;

namespace CachedPathSuggestBoxDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly CombinedSuggest combinedSuggest = new CombinedSuggest();
		private readonly FastObservableCollection<object> listQueryResult = new FastObservableCollection<object>();
		public MainWindow()
		{
			InitializeComponent();

			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= MainWindow_Loaded;

			this.DataContext = new AppViewModel();
		}
	}
}
