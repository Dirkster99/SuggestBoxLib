using System;
using System.IO;

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	/// <summary>
	/// Implements a simple path information object to keep track of its full name and path etc.
	/// </summary>
	public class PathInformation : IEquatable<PathInformation>
	{
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="argValue"></param>
		public PathInformation(string argValue)
		{
			Name = new DirectoryInfo(argValue).Parent == null ? argValue :
				argValue.EndsWith("\\") ? Path.GetDirectoryName(argValue) :
				Path.GetFileName(argValue);

			FullName = argValue;
		}

		/// <summary>
		/// Gets the name of a directory (without the path)
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the full name (including path) for a directory
		/// </summary>
		public string FullName { get; }


		public bool Equals(PathInformation other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Name == other.Name && FullName == other.FullName;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == this.GetType() && Equals((PathInformation)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, FullName);
		}
	}
}