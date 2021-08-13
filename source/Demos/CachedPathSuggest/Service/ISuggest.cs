using System.Collections.Generic;
using System.Threading.Tasks;
using CachedPathSuggest.ViewModels;

namespace CachedPathSuggest.Service
{
    /// <summary>
    ///     Defines an interface to make similarity based suggestions for a given string.
    /// </summary>
    public interface IAsyncSuggest
    {
        /// <summary>
        ///     Makes suggestions based on <see cref="queryThis" />
        /// </summary>
        Task<IReadOnlyCollection<BaseItem>?> SuggestAsync(string queryThis);
    }
}