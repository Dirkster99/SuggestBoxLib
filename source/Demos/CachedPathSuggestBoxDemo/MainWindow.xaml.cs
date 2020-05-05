
using System.Linq;
using System.Windows;
using CachedPathSuggestBoxDemo.Infrastructure;

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

			this.DiskPathSuggestBox.TextChangedCommand = new RelayCommand<object>(Execute); ;
			this.DiskPathSuggestBox.ItemsSource = listQueryResult;
			this.DiskPathSuggestBox.DataContext = this;

		}

		/// <summary>
		/// Need to bind to text in order to generate binding-expression that is then used for displaying validation errors
		/// </summary>
		public string Text { get; set; }

		private async void Execute(object p)
		{
			// We want to process empty strings here as well
			if (!(p is string newText))
				return;

			var suggestions = (await combinedSuggest.MakeSuggestions(newText))?.ToArray();
			listQueryResult.Clear();
			if (suggestions == null)
			{
				this.DiskPathSuggestBox.ValidText = false;
				return;
			}
			this.DiskPathSuggestBox.ValidText = true;
			listQueryResult.AddItems(suggestions);
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			combinedSuggest.CachedPathInformationSuggest.Insert(Text);
		}
	}
}
