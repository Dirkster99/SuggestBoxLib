using CachedPathSuggestBoxDemo.Infrastructure;
using CachedPathSuggestBoxDemo.ViewModels.Base;
using CachedPathSuggestBoxDemo.ViewModels.List;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CachedPathSuggestBoxDemo.ViewModels
{
	/// <summary>
	/// Implements a viewmodel that should be attached to the MainWindow's DataContext.
	/// </summary>
	class AppViewModel : Base.ViewModelBase
	{
		#region fields
		private readonly CombinedSuggest combinedSuggest;
		private readonly FastObservableCollection<BaseItem> listQueryResult;
		private ICommand _TextChangedCommand;
		private bool _ValidText;
		private ICommand _AddBookmarkCommandCommand;
		private ICommand _RemoveBookmarkCommand;
		#endregion fields

		#region ctors
		public AppViewModel()
		{
			combinedSuggest = new CombinedSuggest();
			listQueryResult = new FastObservableCollection<BaseItem>();
		}
		#endregion ctors

		#region properties
		public IEnumerable<BaseItem> ListQueryResult => listQueryResult;

		public ICommand TextChangedCommand
		{
			get
			{
				if (_TextChangedCommand == null)
					_TextChangedCommand = new RelayCommand<object>(
						(p) => TextChangedCommand_Execute(p));

				return _TextChangedCommand;
			}
		}

		public ICommand AddBookmarkCommand
		{
			get
			{
				if (_AddBookmarkCommandCommand == null)
					_AddBookmarkCommandCommand = new RelayCommand<object>(
						(p) => AddBookmarkCommand_Executed(p),
						(p) => AddBookmarkCommand_CanExecuted(p));

				return _AddBookmarkCommandCommand;
			}
		}

		public ICommand RemoveBookmarkCommand
		{
			get
			{
				if (_RemoveBookmarkCommand == null)
					_RemoveBookmarkCommand = new RelayCommand<object>(
						(p) => RemoveBookmarkCommand_Executed(p),
						(p) => RemoveBookmarkCommand_CanExecuted(p));

				return _RemoveBookmarkCommand;
			}
		}

		public bool ValidText
		{
			get => _ValidText;

			set
			{
				if (_ValidText != value)
				{
					_ValidText = value;
					NotifyPropertyChanged(() => ValidText);
				}
			}
		}
		#endregion properties

		#region methods
		private async void TextChangedCommand_Execute(object p)
		{
			// We want to process empty strings here as well
			if (!(p is string newText))
				return;

			var suggestions = (await combinedSuggest.MakeSuggestions(newText))?.ToArray();
			listQueryResult.Clear();
			if (suggestions == null)
			{
				ValidText = false;
				return;
			}
			ValidText = true;
			listQueryResult.AddItems(suggestions);
		}

		#region Add Bookmark Command
		private void AddBookmarkCommand_Executed(object textParam)
		{
			var text = textParam as string;

			if (string.IsNullOrEmpty(text)) return;

			combinedSuggest.InsertCachedSuggestion(text);
		}

		private bool AddBookmarkCommand_CanExecuted(object p)
		{
			string key = p as string;

			if (string.IsNullOrEmpty(key))
				return false;

			return true;
		}
		#endregion Add Bookmark Command

		#region Remove Bookmark Command
		private void RemoveBookmarkCommand_Executed(object p)
		{
			string key = p as string;

			if (string.IsNullOrEmpty(key))
				return;

			combinedSuggest.DeleteCachedSuggestion(key);
		}

		private bool RemoveBookmarkCommand_CanExecuted(object p)
		{
			string key = p as string;

			if (string.IsNullOrEmpty(key))
				return false;

			return true;
		}
		#endregion Remove Bookmark Command
		#endregion methods
	}
}
