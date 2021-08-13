using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CachedPathSuggest.Service;
using CachedPathSuggest.ViewModels;
using CachedPathSuggestBox.Demo.Infrastructure;
using Infrastructure;

namespace CachedPathSuggestBox.Demo.ViewModel
{
    using Command = RelayCommand<RoutedPropertyChangedEventArgs<string>>;

    /// <summary>
    ///     Implements a viewmodel that should be attached to the MainWindow's DataContext.
    /// </summary>
    internal class AppViewModel : ViewModelBase
    {
        private readonly Command addBookmarkCommand;
        private readonly Command removeBookmarkCommand;
        private string text = string.Empty;

        public AppViewModel()
        {
            var combinedAsyncSuggest = new CombinedAsyncSuggest();

            TextChangedCommand = new Command(async eventArgs =>
            {
                if (eventArgs == null)
                    return;
                QueryResults.Clear();
                QueryResults.Add(new BaseItem("Loading..."));
                var suggestions = (await combinedAsyncSuggest.SuggestAsync(eventArgs.NewValue))?.ToArray();
                if (suggestions != null)
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        QueryResults.Clear();
                        QueryResults.AddItems(suggestions);
                    });
            });

            addBookmarkCommand = new Command(_ =>
            {
                combinedAsyncSuggest.AddCachedSuggestion(this.text);
                addBookmarkCommand.RaiseCanExecute();
                removeBookmarkCommand.RaiseCanExecute();
            }, _ => Path.IsPathRooted(text) && combinedAsyncSuggest.ContainsSuggestion(text) == false);


            removeBookmarkCommand = new Command(_ =>
            {
                combinedAsyncSuggest.RemoveCachedSuggestion(this.text);
                addBookmarkCommand.RaiseCanExecute();
                removeBookmarkCommand.RaiseCanExecute();
            }, _ => combinedAsyncSuggest.ContainsSuggestion(text));

            RemoveBookmarkCommand = removeBookmarkCommand;
            AddBookmarkCommand = addBookmarkCommand;

            PropertyChanged += AppViewModel_PropertyChanged;
        }

        public FastObservableCollection<BaseItem> QueryResults { get; } = new(new[] {new BaseItem("Enter a file-system-path or the Space key")});

        public ICommand TextChangedCommand { get; }

        public ICommand AddBookmarkCommand { get; }

        public ICommand RemoveBookmarkCommand { get; }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                NotifyPropertyChanged();
            }
        }

        private void AppViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Text) && Text != null)
            {
                addBookmarkCommand.RaiseCanExecute();
                removeBookmarkCommand.RaiseCanExecute();
            }
        }
    }
}