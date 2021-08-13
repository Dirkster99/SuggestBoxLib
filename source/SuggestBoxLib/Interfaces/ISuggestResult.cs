namespace SuggestBoxLib.Interfaces
{
	using System.Collections.Generic;

	/// <summary>
	/// Defines properties and methods of an object that is used to generate a list
	/// of suggestion results and whether the given path was considered as valid or not.
	///
	/// This type of object is typically used by a <see cref="IAsyncSuggest"/> object.
	/// </summary>
	public interface ISuggestResult
	{
		/// <summary>
		/// Gets a collection of suggestions based on a given input.
		/// </summary>
		IReadOnlyCollection<object> Suggestions { get; }

		/// <summary>
		/// Gets whether the given input was considered as valid.
		/// </summary>
		bool IsValid { get; }
    }
}