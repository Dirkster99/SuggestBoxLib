#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	/// <summary>
	/// Wraps a LiteDB and a FileSystem data provider to generate similarity based suggestions
	/// for a given string.
	/// </summary>
	internal class CombinedSuggest : ISuggest
	{
		private readonly DirectorySuggest directorySuggest = new DirectorySuggest();

		private readonly CachedPathInformationSuggest CachedPathInformationSuggest = new CachedPathInformationSuggest();

		/// <summary>
		/// Gets a list of combined suggestions based on string similarity from the:
		/// 1) cached entries (bookmarks) and
		/// 2) file system data provider.
		/// </summary>
		/// <param name="queryThis"></param>
		/// <returns>
		/// Both types (cached entries and file system provider entries) of entries are returned
		/// in a single list. The list contains a separator item (if both types of items are returned).
		/// The separator item can be used for visual enhancement when displaying the list.
		/// </returns>
		public async Task<IEnumerable<ViewModels.List.BaseItem>?> MakeSuggestions(string queryThis)
		{
			var suggestions1 = (await CachedPathInformationSuggest.MakeSuggestions(queryThis))?.ToArray();

			var suggestions2 = (await directorySuggest.MakeSuggestions(queryThis))?.ToArray();

			if (suggestions2?.Length == 0 && suggestions1?.Any() == false)
				return null;

			if (suggestions2?.Length != 0 && suggestions1?.Any() == false)
				return suggestions2;

			if (suggestions2?.Length == 0 && suggestions1?.Any() == true)
				return suggestions1;

			// Insert a seperator between bookmarked suggestions and suggestions from file system
			return suggestions1
				.Concat(new ViewModels.List.BaseItem[1] { new ViewModels.List.ItemSeperator() })
				.Concat(suggestions2);
		}

		/// <summary>
		/// Insert a new suggestion into the available list of suggestions
		/// </summary>
		/// <param name="text"></param>
		public void InsertCachedSuggestion(string text)
		{
			CachedPathInformationSuggest.Insert(text);
		}

		/// <summary>
		/// Delete a suggestion in the available list of suggestions
		/// </summary>
		/// <param name="text"></param>
		public void DeleteCachedSuggestion(string text)
		{
			CachedPathInformationSuggest.Delete(text);
		}
	}
}