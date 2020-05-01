#nullable enable

namespace SuggestBoxTestLib.DataSources
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class DirectorySuggestSource
    {
        #region fields
        private readonly Dictionary<string, CancellationTokenSource> _Queue;
        private readonly SemaphoreSlim _SlowStuffSemaphore;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectorySuggestSource()
        {
            _Queue = new Dictionary<string, CancellationTokenSource>();
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
        }
        #endregion ctors

        public async Task<IEnumerable<object>?> MakeSuggestions(string queryThis)
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



            static IEnumerable<object> EnumerateSubDirs(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return EnumerateLogicalDrives();

                var subDirs = EnumerateLogicalDriveOrSubDirs(input, input);

                return subDirs ?? Get();

                // Find last separator and list directories underneath
                // with * search-pattern
                IEnumerable<object> Get()
                {
                    int sepIdx = input.LastIndexOf('\\');

                    if (sepIdx >= input.Length) return EnumerateLogicalDrives();
                    string folder = input.Substring(0, sepIdx + 1);
                    string searchPattern = input.Substring(sepIdx + 1) + "*";
                    string[]? directories = null;
                    try
                    {
                        directories = System.IO.Directory.GetDirectories(folder, searchPattern);
                    }
                    catch
                    {
                        // Catch invalid path exceptions here ...
                    }

                    if (directories == null) return EnumerateLogicalDrives();
                    var dirs = new List<object>();

                    foreach (var t in directories)
                        dirs.Add(new { Header = t, Value = t });

                    return dirs;
                }
            }

            static IEnumerable<object> EnumerateDrives(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return EnumerateLogicalDrives();

                return EnumeratePaths(input) ?? EnumerateLogicalDrives();


                static IEnumerable<object>? EnumeratePaths(string input) => input.Length switch
                    {
                        1 when char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z' =>
                        EnumerateLogicalDriveOrSubDirs(input + ":\\", input),

                        2 when char.ToUpper(input[1]) == ':' &&
                               char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z' =>
                        EnumerateLogicalDriveOrSubDirs(input + "\\", input),

                        2 => new List<object>(),

                        _ when (char.ToUpper(input[1]) == ':' &&
                                char.ToUpper(input[2]) == '\\' &&
                                char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z') =>
                        // Check if we know this drive and list it with sub-folders if we do
                        EnumerateLogicalDriveOrSubDirs(input, input),
                        _ => new List<object>()
                    };
            }


            static IEnumerable<object> EnumerateLogicalDrives()
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

                    yield return new {Header = header, Value = driveName};
                }
            }

            static IEnumerable<object>? EnumerateLogicalDriveOrSubDirs(string testDrive, string input)
            {
                return System.IO.Directory.Exists(testDrive) != true ?
                    null :
                    GetLogicalDriveOrSubDirs2(testDrive, input);

                static IEnumerable<object>  GetLogicalDriveOrSubDirs2(string testDrive, string input)
                {
                    // List the drive itself if there was only 1 or 2 letters
                    // since this is not a valid drive and we don'nt know if the user
                    // wants to go to the drive or a folder contained in it
                    if (input.Length <= 2)
                        yield return new {Header = testDrive, Value = testDrive};

                    // and list all sub-directories of that drive
                    foreach (var item in System.IO.Directory.GetDirectories(testDrive))
                        yield return new {Header = item, Value = item};
                }
            }
        }
    }
}
