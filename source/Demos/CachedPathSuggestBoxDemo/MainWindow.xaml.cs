using System;
using System.Collections.Generic;
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
        DirectorySuggestSource source = new DirectorySuggestSource();
        FastObservableCollection<object> _ListQueryResult = new FastObservableCollection<object>();
        SuggestionProvider folderSuggestionProvider =new SuggestionProvider();

        public MainWindow()
        {
            InitializeComponent();

            this.DiskPathSuggestBox.TextChangedCommand = new RelayCommand<object>(Execute); ;
            this.DiskPathSuggestBox.ItemsSource = _ListQueryResult;
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

            var suggestions1 = folderSuggestionProvider.Collection.Where(a => a.Name.Contains(newText, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            var suggestions = await source.MakeSuggestions(newText);

            _ListQueryResult.Clear();
            if (suggestions == null&& suggestions1.Any()==false)
                return;

            var enumerable = suggestions1
                                        .Select(a => new { Header = a.FullName, Value = a.FullName })
                                        .Concat(suggestions as object[] ?? suggestions?? new List<object>())
                                        .ToArray();

            this.DiskPathSuggestBox.ValidText = enumerable.Any();
            _ListQueryResult.AddItems(enumerable);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            folderSuggestionProvider.Insert(Text);
        }
    }
}


