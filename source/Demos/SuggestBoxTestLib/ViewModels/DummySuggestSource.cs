using SuggestBoxLib.Model;

namespace SuggestBoxTestLib.ViewModels
{
    using SuggestBoxLib.Interfaces;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class DummySuggestAsync : IAsyncSuggest
    {
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="location"/> object.
        ///
        /// This sample is really easy because it simply takes the input
        /// string and add an output as suggestion to the given input.
        ///
        /// This always returns 2 suggestions.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<ISuggestResult> SuggestAsync(string input)
        {
            // returns a collection of anynymous objects
            // each with a Header and Value property
            var result = new SuggestResult(new[]{
                new  { Header = input + "-add xyz", Value = input + "xyz" },
                new { Header = input + "-add abc", Value = input + "abc" }});

            return Task.FromResult<ISuggestResult>(result);
        }
    }
}