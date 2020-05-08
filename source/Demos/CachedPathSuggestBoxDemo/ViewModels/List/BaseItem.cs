namespace CachedPathSuggestBoxDemo.ViewModels.List
{
	/// <summary>
	/// This class is the parent root class of other lsit item related classes.
	/// 
	/// It exists simple for methods to produce something like IEnmerable{<see cref="BaseItem"/>}
	/// where 'BaseItem' cen be any class inheriting from <see cref="BaseItem"/>.
	/// 
	/// This enables us to use Converters, DataTemplates or other tools in WPF to display different
	/// UI bits for different types of classes.
	/// </summary>
	public class BaseItem : Base.ViewModelBase
	{
		private bool _IsHitTestVisible;

		public BaseItem()
		{
			IsHitTestVisible = true;
		}

		/// <summary>
		/// Gets/sets a property that indicates to the UI whether the resulting list item
		/// should be clickable to process data or not (a separator is part of the list but should not be clicked).
		/// </summary>
		public bool IsHitTestVisible
		{
			get { return _IsHitTestVisible; }

			set
			{
				if (_IsHitTestVisible != value)
				{
					_IsHitTestVisible = value;
					NotifyPropertyChanged(() => IsHitTestVisible);
				}
			}
		}
	}
}
