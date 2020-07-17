namespace CachedPathSuggestBoxDemo.ViewModels.List
{
	/// <summary>
	/// Implements a simple viewmodel for an item with a header and a value.
	///
	/// This type of item is usually useful in a WPF list of items where quickly
	/// different types of items appear with a need for displaying their content
	/// with a different UI (eg.: using a Converter or DataTemplate).
	/// </summary>
	public class Item : BaseItem
	{
		#region constructors

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="header">String that should be displayed for this list item</param>
		/// <param value="value">string that should be used as a (unique) id to identify this item
		/// within a collection of items.</param>
		internal Item(string header, string value)
			: this()
		{
			Header = header;
			Value = value;
		}

		/// <summary>
		/// Hidden class constructor
		/// </summary>
		protected Item()
			: base()
		{
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets the header string that should be displayed for this list item.
		/// </summary>
		public string Header { get; protected set; }

		/// <summary>
		/// Gets the name string that should be used as a (unique) id to identify this item
		/// within a collection of items.
		/// </summary>
		public string Value { get; protected set; }

		#endregion properties
	}
}