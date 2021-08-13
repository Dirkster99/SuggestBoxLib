using System.Collections.Generic;
using SuggestBoxLib.Interfaces;

namespace SuggestBoxLib.Model
{
    /// <summary>
    /// Models a result class for the drop-down portion of the SuggestionBox.
    /// Each suggestion source <see cref="IAsyncSuggest"/> returns one of these objects
    /// containing both
    /// a list of suggestions and
    /// whether the input is valid.
    /// </summary>
    public class SuggestResult : ISuggestResult
    {
        public SuggestResult(IReadOnlyCollection<object> collection, bool isValid =true)
        {
            Suggestions = collection;
            IsValid = isValid;
        }

        /// <inheritdoc cref="Suggestions"/>
        public IReadOnlyCollection<object> Suggestions { get; }

		/// <inheritdoc cref="IsValid"/>
		public bool IsValid { get; }
    }
}