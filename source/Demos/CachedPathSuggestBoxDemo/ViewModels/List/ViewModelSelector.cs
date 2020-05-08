using System.Windows;
using System.Windows.Controls;

namespace CachedPathSuggestBoxDemo.ViewModels.List
{
	public class ViewModelSelector : DataTemplateSelector
	{
		public DataTemplate ListItem { get; set; }

		public DataTemplate Separator { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is ViewModels.List.Item)
				return this.ListItem;

			if (item is ViewModels.List.ItemSeperator)
				return this.Separator;

			return base.SelectTemplate(item, container);
		}
	}
}
