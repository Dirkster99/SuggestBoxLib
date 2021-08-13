using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CachedPathSuggest.Infrastructure;
using CachedPathSuggest.ViewModels;

namespace CachedPathSuggest.Service
{
    /// <summary>
    ///     Wraps a LiteDB and a FileSystem data provider to generate similarity based suggestions
    ///     for a given string.
    ///     Defines a suggestion object to generate suggestions based on sub entries of specified string.
    /// </summary>
    public class DirectoryAsyncSuggest : IAsyncSuggest
    {
        private CancellationTokenSource tokenSource = new();

        public DirectoryAsyncSuggest()
        {
#pragma warning disable 4014
            // This speeds-up subsequent calls to SuggestAsync
            SuggestAsync(string.Empty);
#pragma warning restore 4014
        }

        /// <summary>
        ///     Class constructor
        /// </summary>
        public async Task<IReadOnlyCollection<BaseItem>?> SuggestAsync(string queryThis)
        {
            tokenSource.Cancel();

            tokenSource = new CancellationTokenSource();
            var ty = Task.Run(() =>
                (string.IsNullOrWhiteSpace(queryThis)
                    ? DirectoryHelper.EnumerateLogicalDrives().ToArray()
                    : DirectoryHelper.EnumerateSubDirectories(queryThis)?.ToArray() ??
                      Array.Empty<(string name, string? vi)>())
                .Select(r => new PathInformation(r.path, r.label)).ToArray(), tokenSource.Token);

            return await ty;
        }
    }
}