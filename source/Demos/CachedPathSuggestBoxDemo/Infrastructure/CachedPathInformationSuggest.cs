#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CachedPathSuggestBoxDemo.Infrastructure
{


    public class CachedPathInformationSuggest :ISuggest
    {
        public void Insert(string path)
        {
            var fileInfo =new FileInfo(path);
            if (fileInfo.Exists)
            {
                LiteRepository.Instance.Insert(fileInfo.FullName);
            }

            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists) return;
            LiteRepository.Instance.Insert(directoryInfo.FullName);
        }

        /// <summary>
        /// Makes suggestions of paths based on match between query and the name of the path.
        /// Only returns latest 3 results. Newest first.
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
        public async Task<IEnumerable<object>?> MakeSuggestions(string queryThis)
        {
            return await Task.Run(() => MakeSuggestionsPrivate(queryThis).ToArray());

            IEnumerable<object> MakeSuggestionsPrivate(string queryThis) => from item in GetPathInformations(queryThis)
                where item.Name.Contains(queryThis, StringComparison.CurrentCultureIgnoreCase)
                select new { Header = item.FullName, Value = item.FullName };

            static PathInformation[] GetPathInformations(string key)
            {
                return LiteRepository.Instance.Filter(key).OrderByDescending(a => a.Value).Take(5).Select(a => new PathInformation(a.Key)).ToArray();
            }
        }
    }
}