#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
    public interface ISuggest
    {
        /// <summary>
        /// Makes suggestions based on <see cref="queryThis"/>
        /// </summary>
        Task<IEnumerable<object>?> MakeSuggestions(string queryThis);
    }
}