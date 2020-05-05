#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	public sealed class LiteRepository
	{
		private const string DbPath = @"..\..\..\Data\KeyValue.litedb";

		// Collection needs a name because object, keyvaluepair<string,string> is generic.
		private const string CollectionName = "collection";

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static LiteRepository()
		{
		}

		public void Insert(string k, string? collectionName = null)
		{
			using var db = new LiteDatabase(DbPath);
			var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);
			col.Upsert(new KeyValuePair<string, DateTime>(k, DateTime.Now));
		}

		public IEnumerable<KeyValuePair<string, DateTime>> Filter(string key, string? collectionName = null)
		{
			using var db = new LiteDatabase(DbPath);
			var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);
			return string.IsNullOrWhiteSpace(key) ?
				col.Query().ToArray() :
				col.Find(Query.Contains(nameof(KeyValuePair<string, DateTime>.Key), key)).ToArray();
		}

		private LiteRepository()
		{
			Directory.GetParent(DbPath).Create();

			// Id needed to ensure Upsert works.
			BsonMapper.Global.Entity<KeyValuePair<string, string>>()
				.Id(x => x.Value);

		}

		public static LiteRepository Instance { get; } = new LiteRepository();
	}
}