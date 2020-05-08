using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace CachedPathSuggestBoxDemo.Infrastructure
{
	/// <summary>
	/// Wraps a LiteDB and a FileSystem data provider to generate similarity based suggestions
	/// for a given string.
	/// 
	/// Defines a suggestion object to generate suggestions based on sub entries of specified string.
	/// </summary>
	public class DirectorySuggest : ISuggest
	{
		#region fields
		private readonly Dictionary<string, CancellationTokenSource> _Queue;
		private readonly SemaphoreSlim _SlowStuffSemaphore;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public DirectorySuggest()
		{
			_Queue = new Dictionary<string, CancellationTokenSource>();
			_SlowStuffSemaphore = new SemaphoreSlim(1, 1);
		}
		#endregion ctors

		public async Task<IEnumerable<ViewModels.List.BaseItem>?> MakeSuggestions(string queryThis)
		{
			// Cancel current task(s) if there is any...
			var queueList = _Queue.Values.ToList();

			foreach (var t in queueList)
				t.Cancel();

			var tokenSource = new CancellationTokenSource();
			_Queue.Add(queryThis, tokenSource);

			// Make sure the task always processes the last input but is not started twice
			await _SlowStuffSemaphore.WaitAsync(tokenSource.Token);
			try
			{
				// There is more recent input to process so we ignore this one
				if (_Queue.Count <= 1)
					return await (string.IsNullOrEmpty(queryThis)
						? Task.FromResult(EnumerateSubDirs(queryThis))
						: Task.FromResult(queryThis.Length <= 3 ? EnumerateDrives(queryThis) : EnumerateSubDirs(queryThis)));
				_Queue.Remove(queryThis);
				return null;

			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.Message);
			}
			finally
			{
				_Queue.Remove(queryThis);
				_SlowStuffSemaphore.Release();
			}

			return null;

			static IEnumerable<ViewModels.List.Item>? EnumerateSubDirs(string input)
			{
				if (string.IsNullOrEmpty(input))
					return EnumerateLogicalDrives();

				var subDirs = EnumerateLogicalDriveOrSubDirs(input, input);

				return subDirs != null ? subDirs.Any() ? subDirs : Get() : null;

				// Find last separator and list directories underneath
				// with * search-pattern
				IEnumerable<ViewModels.List.Item> Get()
				{
					int sepIdx = input.LastIndexOf('\\');

					if (sepIdx >= input.Length) return EnumerateLogicalDrives();
					string folder = input.Substring(0, sepIdx + 1);
					string searchPattern = input.Substring(sepIdx + 1) + "*";
					string[]? directories = null;
					try
					{
						directories = Directory.GetDirectories(folder, searchPattern);
					}
					catch
					{
						// Catch invalid path exceptions here ...
					}

					if (directories == null) return EnumerateLogicalDrives();
					var dirs = new List<ViewModels.List.Item>();

					foreach (var t in directories)
						dirs.Add(new ViewModels.List.Item (t, t ));

					return dirs;
				}
			}

			static IEnumerable<ViewModels.List.Item> EnumerateDrives(string input)
			{
				if (string.IsNullOrEmpty(input))
					return EnumerateLogicalDrives();

				return EnumeratePaths(input) ?? EnumerateLogicalDrives();

				static IEnumerable<ViewModels.List.Item>? EnumeratePaths(string input) => input.Length switch
				{
					1 when char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z' =>
					EnumerateLogicalDriveOrSubDirs(input + ":\\", input),

					2 when char.ToUpper(input[1]) == ':' &&
						   char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z' =>
					EnumerateLogicalDriveOrSubDirs(input + "\\", input),

					2 => new List<ViewModels.List.Item>(),

					_ when (char.ToUpper(input[1]) == ':' &&
							char.ToUpper(input[2]) == '\\' &&
							char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z') =>
					// Check if we know this drive and list it with sub-folders if we do
					EnumerateLogicalDriveOrSubDirs(input, input),
					_ => new List<ViewModels.List.Item>()
				};
			}

			static IEnumerable<ViewModels.List.Item> EnumerateLogicalDrives()
			{
				foreach (var driveName in Environment.GetLogicalDrives()
					.Where(driveName => string.IsNullOrEmpty(driveName) == false))
				{
					string header;

					try
					{
						DriveInfo d = new DriveInfo(driveName);
						header = string.IsNullOrEmpty(d.VolumeLabel) == false
							? $"{d.VolumeLabel} ({d.Name})"
							: driveName;
					}
					catch
					{
						header = driveName;
					}

					yield return new ViewModels.List.Item (header, driveName );
				}
			}

			static IEnumerable<ViewModels.List.Item>? EnumerateLogicalDriveOrSubDirs(string testDrive, string input)
			{
				return System.IO.Directory.Exists(testDrive) ?
					GetDirectories(testDrive) is { } array ?
					GetLogicalDriveOrSubDirs2(testDrive, input, array) :
					null :
					null;

				static IEnumerable<ViewModels.List.Item>? GetLogicalDriveOrSubDirs2(string testDrive, string input, IEnumerable<string> directories)
				{
					// List the drive itself if there was only 1 or 2 letters
					// since this is not a valid drive and we don'nt know if the user
					// wants to go to the drive or a folder contained in it
					if (input.Length <= 2)
						yield return new ViewModels.List.Item(testDrive, testDrive );

					foreach (var item in directories)
						yield return new ViewModels.List.Item(item, item );
				}

				static string[]? GetDirectories(string testDrive)
				{
					string[] directories;
					try
					{
						directories = Directory.GetDirectories(testDrive);
					}
					catch (UnauthorizedAccessException)
					{
						return null;
					}

					return directories;
				}
			}
		}
	}
}
