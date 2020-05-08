namespace CachedPathSuggestBoxDemo.ViewModels.List
{
	/// <summary>
	/// Implements a simple viewmodel for an item that should act as separator
	/// between 2 sub-sequent blocks of 2 different types of items.
	/// 
	/// This type of item is usually useful in a WPF list of items where quickly
	/// different types of items appear with a need for displaying their content
	/// with a different UI (eg.: using a Converter or DataTemplate).
	/// </summary>
	public class ItemSeperator : BaseItem
	{
		/// <summary>
		/// Class constructor
		/// </summary>
		public ItemSeperator()
			: base()
		{
			// a separator is part of the list but should not be clicked
			IsHitTestVisible = false;
		}
	}
}
