namespace SuggestBoxTestLib.Views
{
	using SuggestBoxLib.Events;
	using SuggestBoxTestLib.ViewModels.Base;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Threading;

	/// <summary>
	/// Interaction logic for DemoView.xaml
	/// </summary>
	public partial class DemoView : UserControl
	{
		private ICommand _RecentListCommand;

		/// <summary>
		/// Class constructor
		/// </summary>
		public DemoView()
		{
			InitializeComponent();

			DiskPathSuggestBox.NewLocationRequestEvent += Control_SuggestBox_NewLocationRequestEvent;

			PART_SuggestComboBox.SelectionChanged += PART_SuggestComboBox_SelectionChangedAsync;
		}

		public ICommand RecentListCommand
		{
			get
			{
				if (_RecentListCommand == null)
				{
					_RecentListCommand = new RelayCommand<object>((p) =>
					{
						if (PART_SuggestComboBox != null)
						{
							PART_SuggestComboBox.SelectedItem = null;
							PART_SuggestComboBox.IsDropDownOpen = true;
						}
					});
				}

				return _RecentListCommand;
			}
		}

		private void PART_SuggestComboBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
		{
			if ((sender is ComboBox) == false)
				return;

			if (e.AddedItems.Count > 0)
			{
				string input = null;
				input = e.AddedItems[0] as string;
				if (string.IsNullOrEmpty(input) == true)
				{
					var item = e.AddedItems[0] as ComboBoxItem;
					if (item != null)
						input = item.Content as string;
				}

				if (string.IsNullOrEmpty(input) == false)
				{
					DiskPathSuggestComboBox.Text = input;
					DiskPathSuggestComboBox.SelectAll();
				}
			}

			Application.Current.Dispatcher.Invoke(() =>
			{
				DiskPathSuggestComboBox.Focus();
				Keyboard.Focus(DiskPathSuggestComboBox);
			}, DispatcherPriority.ApplicationIdle);
		}

		/// <summary>
		/// Method executes when the SuggestionBox signals that editing location
		/// has been OK'ed (user pressed enter) or cancel'ed (user pressed Escape).
		///
		/// These signals are then recorded and processed via IsSwitchOn property
		/// handler which can also be invoked via Toggle Button which is processed
		/// as OK.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Control_SuggestBox_NewLocationRequestEvent(object sender,
																SuggestBoxLib.Events.NextTargetLocationArgs e)
		{
			// The user requests a new location via SuggestBox textbox key gesture:
			// Enter  -> OK
			// Escape -> Cancel
			string action = "Unknown";
			switch (e.EditResult.Result)
			{
				case EditPathResult.Cancel:
					action = "Cancel";
					break;

				case EditPathResult.OK:
					action = "OK";
					break;

				default:
					throw new System.NotImplementedException(e.EditResult.Result.ToString());
			}

			if (string.IsNullOrEmpty(e.EditResult.NewLocation) == false)
			{
				NewLocationRequestEventDisplay.Text =
									string.Format("({0}) '{1}'", action, e.EditResult.NewLocation);
			}
			else
				NewLocationRequestEventDisplay.Text = string.Format("{0}", action);
		}
	}
}