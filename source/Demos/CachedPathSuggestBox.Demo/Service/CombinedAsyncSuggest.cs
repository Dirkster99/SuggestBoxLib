using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachedPathSuggest.Infrastructure;
using CachedPathSuggest.ViewModels;
using SuggestBoxLib.Interfaces;
using SuggestBoxLib.Model;

namespace CachedPathSuggestBox.Demo.Service
{
    /// <summary>
    ///     Wraps a LiteDB and a FileSystem data provider to generate similarity based suggestions
    ///     for a given string.
    /// </summary>
    public class CombinedAsyncSuggest : IAsyncSuggest
    {
        private readonly CachedAsyncSuggest cachedAsyncSuggest = new(new LiteRepository());
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
        public async Task<ISuggestResult> SuggestAsync(string queryThis)
        {
            var cachedSuggestions = await cachedAsyncSuggest.SuggestAsync(queryThis);
            var directorySuggestions = await directoryAsyncSuggest.SuggestAsync(queryThis);
            var suggestions = (cachedSuggestions, directorySuggestions) switch
            {
                (null, null) => null,
                ({ Suggestions: { Count: 0 } }, { Suggestions: { Count: 0 } }) => null,
                ({ Suggestions: { Count: 0 } }, { Suggestions: { Count: > 0 } }) => directorySuggestions.Suggestions,
                ({ Suggestions: { Count: > 0 } }, { Suggestions: { Count: 0 } }) => cachedSuggestions.Suggestions,
                ({ Suggestions: { Count: > 0 } c }, { Suggestions: { Count: > 0 } d }) => c

                    .Concat(new[] { new ItemSeparator() }).Concat(d).ToArray(),
                _ => throw new ArgumentOutOfRangeException()
            };
            return new SuggestResult(suggestions, suggestions?.Count > 0);
        }

        /// <summary>
        ///     Insert a new suggestion into the available list of suggestions
        /// </summary>
        /// <param name="text"></param>
        public void AddCachedSuggestion(string text)
        {
            cachedAsyncSuggest.Insert(text);
        }

        /// <summary>
        ///     Delete a suggestion in the available list of suggestions
        /// </summary>
        /// <param name="text"></param>
        public void RemoveCachedSuggestion(string text)
        {
            cachedAsyncSuggest.Delete(text);
        }

        /// <summary>
        ///     Insert a new suggestion into the available list of suggestions
        /// </summary>
        /// <param name="text"></param>
        public bool ContainsSuggestion(string text)
        {
            return cachedAsyncSuggest.Match(text);
        }
    }
}