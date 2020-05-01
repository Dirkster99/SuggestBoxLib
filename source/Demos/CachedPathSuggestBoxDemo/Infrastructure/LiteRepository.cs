using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfApp1
{

    public class SuggestionProvider 
    {
        private Lazy<PathInformation[]> filesCache => new Lazy<PathInformation[]>(() =>
        {
            var folders = LiteRepository.Instance.SelectAll();
            return folders.Select(a => new PathInformation(a.Value)).ToArray();
        });

        public IEnumerable<PathInformation> Collection => filesCache.Value;

        public void Insert(string path)
        {
            var fileInfo =new FileInfo(path);
            if(fileInfo.Exists)
            LiteRepository.Instance.Insert(fileInfo.Name,fileInfo.FullName);

            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
                LiteRepository.Instance.Insert(directoryInfo.Name, directoryInfo.FullName);
        }
    }


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

        public void Insert(string k, string value, string collectionName = null)
        {
            using var db = new LiteDatabase(DbPath);
            var col = db.GetCollection<KeyValuePair<string, string>>(collectionName ??= CollectionName);
            col.Upsert(new KeyValuePair<string, string>(k, value));
        }

        public IEnumerable<KeyValuePair<string, string>> SelectAll(string collectionName = null)
        {
            using var db = new LiteDatabase(DbPath);
            var col = db.GetCollection<KeyValuePair<string, string>>(collectionName ??= CollectionName);
            return col.Query().ToArray();
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



    public class PathInformation : IEquatable<PathInformation>
    {
        public PathInformation(string argValue)
        {
            if (new DirectoryInfo(argValue).Parent == null)
            {
                Name = argValue;
            }
            else
                Name = System.IO.Path.GetFileName(argValue);
            FullName = argValue;
        }

        public string Name { get; }

        public string FullName { get; }


        public bool Equals(PathInformation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && FullName == other.FullName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathInformation)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, FullName);
        }
    }
}