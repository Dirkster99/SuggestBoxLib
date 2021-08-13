using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CachedPathSuggest.Infrastructure;
using CachedPathSuggest.ViewModels;

namespace CachedPathSuggest.Service
{
    /// <summary>
    ///     Wraps a LiteDB to generate previously bookmarked suggestions through similarity for a given string.
    /// </summary>
    public class CachedPathInformationAsyncSuggest : IAsyncSuggest
    {
        private static readonly int NumberOfResultsToReturn = 5;
        private readonly LiteRepository repository;

        public CachedPathInformationAsyncSuggest(LiteRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        ///     Makes suggestions of paths based on match between query and the full-name of the path.
        ///     Only returns latest <see cref="NumberOfResultsToReturn" /> results (newest first).
        /// </summary>
        /// <inheritdoc cref="SuggestAsync" />
        /// <example>
        ///     <see cref="queryThis" /> : doc
        ///     will match with
        ///     c:\\Documents
        ///     t:\\files\store\doC.xaml
        ///     but not
        ///     f:\\do_letters
        ///     g:\\document\lists.ico
        /// </example>
        public async Task<IReadOnlyCollection<BaseItem>?> SuggestAsync(string queryThis)
        {
            return await Task.Run(() => GetPathInformations(queryThis, repository));

            static CachedPathInformation[] GetPathInformations(string key, LiteRepository repository)
            {
                return repository
                    .Filter(key)
                    .Take(NumberOfResultsToReturn)
                    .OrderByDescending(a => a.Value)
                    .Select(a => new CachedPathInformation(a.Value, a.Key))
                    .ToArray();
            }
        }

        public void Insert(string path)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists) repository.Insert(fileInfo.FullName);

            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                throw new Exception("Path does not exist");
            repository.Insert(directoryInfo.FullName);
        }

        internal void Delete(string text)
        {
            repository.Remove(text);
        }

        internal bool Match(string text)
        {
            return repository.Find(text) is not {Key: null};
        }
    }
}