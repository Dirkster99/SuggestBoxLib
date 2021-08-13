#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace CachedPathSuggest.Infrastructure
{
    /// <summary>
    ///     Implements a static service provider for searching and editing a LiteDB file.
    /// </summary>
    public class LiteRepository
    {
        private const string DbPath = @"..\..\..\Data\KeyValue.litedb";

        // Collection needs a name because object, keyvaluepair<string,string> is generic.
        private const string CollectionName = "collection";

        /// <summary>
        ///     Class constructor
        /// </summary>
        public LiteRepository()
        {
            (Directory.GetParent(DbPath) ?? throw new Exception("Can't create directory")).Create();
            BsonMapper.Global.Entity<KeyValuePair<string, DateTime>>().Id(x => x.Key);
        }

        public event Action? Change;

        /// <summary>
        ///     Inserts a new string into the collection of bookmarked strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="collectionName"></param>
        public void Insert(string key, string? collectionName = null)
        {
            using var db = new LiteDatabase(DbPath);
            var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);
            col.Upsert(new KeyValuePair<string, DateTime>(key, DateTime.Now));
            Change?.Invoke();
        }

        /// <summary>
        ///     Inserts a new string into the collection of bookmarked strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="collectionName"></param>
        public void Remove(string key, string? collectionName = null)
        {
            using var db = new LiteDatabase(DbPath);
            var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);

            // Make sure string is already present
            if (col.FindOne(x => x.Key == key) is { } one)
                col.Delete(key);

            Change?.Invoke();
        }

        /// <summary>
        /// </summary>
        public KeyValuePair<string, DateTime>? Find(string key, string? collectionName = null)
        {
            using var db = new LiteDatabase(DbPath);
            var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);
            var one = col.FindOne(x => x.Key == key);
            return one;
        }

        /// <summary>
        ///     Filters the collection of bookmark strings by the given string and
        ///     returns the resulting collection.
        /// </summary>
        public IEnumerable<KeyValuePair<string, DateTime>> Filter(string key, string? collectionName = null)
        {
            using var db = new LiteDatabase(DbPath);
            var col = db.GetCollection<KeyValuePair<string, DateTime>>(collectionName ?? CollectionName);
            return string.IsNullOrWhiteSpace(key)
                ? col.Query().ToArray()
                : col.Find(Query.Contains(nameof(KeyValuePair<string, DateTime>.Key), key)).ToArray();
        }
    }
}