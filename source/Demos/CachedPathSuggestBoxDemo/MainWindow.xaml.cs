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
        private readonly DirectorySuggest directorySuggest = new DirectorySuggest();
        private readonly FastObservableCollection<object> listQueryResult = new FastObservableCollection<object>();
        public readonly CachedPathInformationSuggest cachedPathInformationSuggest =new CachedPathInformationSuggest();

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

            var suggestions2 = await directorySuggest.MakeSuggestions(newText);

            listQueryResult.Clear();
            if (suggestions2 == null)
            {
                this.DiskPathSuggestBox.ValidText = false;
                return;
            }
            this.DiskPathSuggestBox.ValidText = true;

            var suggestions1 = await cachedPathInformationSuggest.MakeSuggestions(newText);

            var enumerable = suggestions1
                                        .Concat(suggestions2)
                                        .ToArray();

            listQueryResult.AddItems(enumerable);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            cachedPathInformationSuggest.Insert(Text);
        }
    }
}


