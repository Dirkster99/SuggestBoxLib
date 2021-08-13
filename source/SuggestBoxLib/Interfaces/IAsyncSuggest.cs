namespace SuggestBoxLib.Interfaces
{
	using System.Threading.Tasks;


	public interface IAsyncSuggest
	{
        /// <param name="input">text input to generate <see cref="Task{ISuggestResult}"/>.</param>
		/// <returns>
		/// a <see cref="ISuggestResult"/> <see cref="Task"/>
        /// </returns>
		Task<ISuggestResult> SuggestAsync(string input);
	}
}