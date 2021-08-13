using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CachedPathSuggestBox.Demo.Service;
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
            AsyncSuggest = new CombinedAsyncSuggest();

            TextChangedCommand = new Command( _ =>
            {
            });

            addBookmarkCommand = new Command(_ =>
            {
                AsyncSuggest.AddCachedSuggestion(text);
                addBookmarkCommand.RaiseCanExecute();
                removeBookmarkCommand.RaiseCanExecute();
            }, _ => Path.IsPathRooted(text) && AsyncSuggest.ContainsSuggestion(text) == false);


            removeBookmarkCommand = new Command(_ =>
            {
                AsyncSuggest.RemoveCachedSuggestion(text);
                addBookmarkCommand.RaiseCanExecute();
                removeBookmarkCommand.RaiseCanExecute();
            }, _ => AsyncSuggest.ContainsSuggestion(text));

            RemoveBookmarkCommand = removeBookmarkCommand;
            AddBookmarkCommand = addBookmarkCommand;

            PropertyChanged += AppViewModel_PropertyChanged;
        }

        public CombinedAsyncSuggest AsyncSuggest { get; set; }

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