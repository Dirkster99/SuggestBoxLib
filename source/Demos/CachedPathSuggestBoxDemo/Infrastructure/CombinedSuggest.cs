#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	class CombinedSuggest : ISuggest
	{
		private readonly DirectorySuggest directorySuggest = new DirectorySuggest();

		public readonly CachedPathInformationSuggest CachedPathInformationSuggest = new CachedPathInformationSuggest();

		public async Task<IEnumerable<object>?> MakeSuggestions(string queryThis)
		{

			var suggestions1 = (await CachedPathInformationSuggest.MakeSuggestions(queryThis))?.ToArray();

			var suggestions2 = (await directorySuggest.MakeSuggestions(queryThis))?.ToArray();

			if (suggestions2 == null && suggestions1?.Any() == false)
			{
				return null;
			}

			return suggestions2 != null ?
				suggestions1?.Concat(suggestions2) :
				suggestions1;
		}
	}
}
