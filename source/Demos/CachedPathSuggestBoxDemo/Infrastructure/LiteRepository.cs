#nullable enable

using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	/// <summary>
	/// Implements a static service provider for searching and editing a LiteDB file.
	/// </summary>
	public sealed class LiteRepository
	{
		#region fields

		private const string DbPath = @"..\..\..\Data\KeyValue.litedb";

		// Collection needs a name because object, keyvaluepair<string,string> is generic.
		private const string CollectionName = "collection";

		#endregion fields

		#region ctors

		/// <summary>
		/// Explicit static constructor to tell C# compiler
		/// not to mark type as beforefieldinit
		/// </summary>
		static LiteRepository()
		{
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		private LiteRepository()
		{
			Directory.GetParent(DbPath).Create();

			// Id needed to ensure Upsert works.
			BsonMapper.Global.Entity<KeyValuePair<string, DateTime>>().Id(x => x.Key);
		}

		#endregion ctors

		/// <summary>
		/// Gets the repository instance from this static instance.
		/// </summary>
		public static LiteRepository Instance { get; } = new LiteRepository();

		/// <summary>
		/// Inserts a new string into the collection of bookmarked strings.
		/// </summary>
		/// <param name="k"></param>
		/// <param name="collectionName"></param>
		public void Insert(string k, string? collectionName = null)
		{
			using var db = new LiteDatabase(DbPath);
			var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);

			// Make sure string is not already present
			col.Upsert(new KeyValuePair<string, DateTime>(k, DateTime.Now));
		}

		/// <summary>
		/// Inserts a new string into the collection of bookmarked strings.
		/// </summary>
		/// <param name="k"></param>
		/// <param name="collectionName"></param>
		public void Remove(string key, string? collectionName = null)
		{
			using var db = new LiteDatabase(DbPath);
			var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);

			// Make sure string is not already present
			if (col.Exists(Query.Contains(nameof(KeyValuePair<string, DateTime>.Key), key)) == false)
				col.Delete(key);
		}

		/// <summary>
		/// Filters the collection of bookmark strings by the given string and
		/// returns the resulting collection.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="collectionName"></param>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, DateTime>> Filter(string key, string? collectionName = null)
		{
			using var db = new LiteDatabase(DbPath);
			var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);
			return string.IsNullOrWhiteSpace(key) ?
				col.Query().ToArray() :
				col.Find(Query.Contains(nameof(KeyValuePair<string, DateTime>.Key), key)).ToArray();
		}
	}
}