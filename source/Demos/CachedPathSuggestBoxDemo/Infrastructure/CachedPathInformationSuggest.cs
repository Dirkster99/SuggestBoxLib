#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	/// <summary>
	/// Wraps a LiteDB to generate previously bookmarked suggestions through similarity for a given string.
	/// </summary>
	public class CachedPathInformationSuggest : ISuggest
	{
		private static readonly int NumberOfResultsReturned = 5;

		public void Insert(string path)
		{
			var fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				LiteRepository.Instance.Insert(fileInfo.FullName);
			}

			var directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists) return;
			LiteRepository.Instance.Insert(directoryInfo.FullName);
		}

		internal void Delete(string text)
		{
			LiteRepository.Instance.Remove(text);
		}

		/// <summary>
		/// Makes suggestions of paths based on match between query and the full-name of the path.
		/// Only returns latest <see cref="NumberOfResultsReturned"/> results (newest first).
		/// </summary>
		/// <inheritdoc cref="MakeSuggestions"/>
		/// <example>
		/// <see cref="queryThis"/> : doc
		/// will match with
		/// c:\\Documents
		/// t:\\files\store\doC.xaml
		/// but not
		/// f:\\do_letters
		/// g:\\document\lists.ico
		/// </example>
		public async Task<IEnumerable<ViewModels.List.BaseItem>?> MakeSuggestions(string queryThis)
		{
			return await Task.Run(() => MakeSuggestionsPrivate().ToArray());

			IEnumerable<ViewModels.List.BaseItem> MakeSuggestionsPrivate() =>
				from item in GetPathInformations(queryThis)
				select new ViewModels.List.Item(item.FullName, item.FullName);

			static PathInformation[] GetPathInformations(string key)
			{
				return LiteRepository.Instance.Filter(key).OrderByDescending(a => a.Value).Take(NumberOfResultsReturned).Select(a => new PathInformation(a.Key)).ToArray();
			}
		}
	}
}