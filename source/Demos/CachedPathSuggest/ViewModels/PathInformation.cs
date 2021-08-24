using System;
using System.IO;

namespace CachedPathSuggest.ViewModels
{
    public class CachedPathInformation : PathInformation
    {
        public CachedPathInformation(DateTime storageDate, string path) : base(path)
        {
            StorageDate = storageDate;
        }

        public DateTime StorageDate { get; }

        public TimeSpan StorageLength => StorageDate - DateTime.Now;
    }

    /// <summary>
    ///     Implements a simple path information object to keep track of its full name and path etc.
    /// </summary>
    public class PathInformation : BaseItem, IEquatable<PathInformation>
    {
        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="information"></param>
        public PathInformation(string path, string? information = null) : base(path)
        {
            Parent = new DirectoryInfo(path).Parent == null ? path :
                path.EndsWith("\\") ? Path.GetDirectoryName(path) ?? string.Empty :
                string.Empty;

            FullName = path;
            Information = information;
        }

        /// <summary>
        ///     Gets the name of a directory (without the path)
        /// </summary>
        public string Parent { get; }

        /// <summary>
        ///     Gets the full name (including path) for a directory
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Any other information e.g volume-information
        /// </summary>
        public string? Information { get; }

        public bool Equals(PathInformation? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Parent == other.Parent && FullName == other.FullName;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PathInformation) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Parent, FullName);
        }
    }
}