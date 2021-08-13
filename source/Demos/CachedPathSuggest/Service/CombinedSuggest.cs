using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachedPathSuggest.Infrastructure;
using CachedPathSuggest.ViewModels;

namespace CachedPathSuggest.Service
{
    /// <summary>
    ///     Wraps a LiteDB and a FileSystem data provider to generate similarity based suggestions
    ///     for a given string.
    /// </summary>
    public class CombinedAsyncSuggest : IAsyncSuggest
    {
        private readonly CachedPathInformationAsyncSuggest
            cachedPathInformationAsyncSuggest = new(new LiteRepository());

        private readonly DirectoryAsyncSuggest directoryAsyncSuggest = new();

        /// <summary>
        ///     Gets a list of combined suggestions based on string similarity from the:
        ///     1) cached entries (bookmarks) and
        ///     2) file system data provider.
        /// </summary>
        /// <param name="queryThis"></param>
        /// <returns>
        ///     Both types (cached entries and file system provider entries) of entries are returned
        ///     in a single list. The list contains a separator item (if both types of items are returned).
        ///     The separator item can be used for visual enhancement when displaying the list.
        /// </returns>
        public async Task<IReadOnlyCollection<BaseItem>?> SuggestAsync(string queryThis)
        {
            var cachedSuggestions = await cachedPathInformationAsyncSuggest.SuggestAsync(queryThis);
            var directorySuggestions = await directoryAsyncSuggest.SuggestAsync(queryThis);
            return (cachedSuggestions, directorySuggestions) switch
            {
                (null, null) => null,
                ({Count: 0}, {Count: 0}) => null,
                ({Count: 0}, {Count: > 0}) => directorySuggestions,
                ({Count: > 0}, {Count: 0}) => cachedSuggestions,
                ({Count: > 0} c, {Count: > 0} d) => c
                    .Concat(new[] {new ItemSeparator()}).Concat(d).ToArray(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        ///     Insert a new suggestion into the available list of suggestions
        /// </summary>
        /// <param name="text"></param>
        public void AddCachedSuggestion(string text)
        {
            cachedPathInformationAsyncSuggest.Insert(text);
        }

        /// <summary>
        ///     Delete a suggestion in the available list of suggestions
        /// </summary>
        /// <param name="text"></param>
        public void RemoveCachedSuggestion(string text)
        {
            cachedPathInformationAsyncSuggest.Delete(text);
        }

        /// <summary>
        ///     Insert a new suggestion into the available list of suggestions
        /// </summary>
        /// <param name="text"></param>
        public bool ContainsSuggestion(string text)
        {
            return cachedPathInformationAsyncSuggest.Match(text);
        }
    }
}