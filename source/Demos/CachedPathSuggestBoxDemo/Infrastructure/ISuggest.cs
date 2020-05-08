#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	/// <summary>
	/// Defines an interface to make similarity based suggestions for a given string.
	/// </summary>
	public interface ISuggest
	{
		/// <summary>
		/// Makes suggestions based on <see cref="queryThis"/>
		/// </summary>
		Task<IEnumerable<ViewModels.List.BaseItem>?> MakeSuggestions(string queryThis);
	}
}