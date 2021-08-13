using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CachedPathSuggest.Infrastructure
{
    internal static class DirectoryHelper
    {
        public static IEnumerable<(string path, string? label)>? EnumerateSubDirectories(string input)
        {
            var subDirs = EnumerateLogicalDriveOrSubDirectories(input);

            // ReSharper disable PossibleMultipleEnumeration
            return subDirs != null
                ? subDirs.Any() 
                ? subDirs.Select(a => (a, (string?)null)) : EnumerateChildren(input)
                : new[] { (input, (string?)"Unauthorized Access") };

            // Find last separator and list directories underneath with * search-pattern
            static IEnumerable<(string, string?)> EnumerateChildren(string input)
            {
                var sepIdx = input.LastIndexOf('\\');

                if (sepIdx >= input.Length)
                    return EnumerateLogicalDrives();

                string folder = input.Substring(0, sepIdx + 1);
                string searchPattern = input.Substring(sepIdx + 1) + "*";
                try
                {
                    return Directory
                            .GetDirectories(folder, searchPattern)
                            .Select(a => (a, (string?)null)).ToList();
                }
                catch
                {
                    // Catch invalid path exceptions here ...
                    return EnumerateLogicalDrives();
                }
            }

            static IEnumerable<string>? EnumerateLogicalDriveOrSubDirectories(string testDrive)
            {
                if (testDrive.Length < 3) return Enumerate(testDrive);

                return Directory.Exists(testDrive) ? TryGetDirectories(testDrive) : Array.Empty<string>();

                static string[]? TryGetDirectories(string testDrive)
                {
                    try
                    {
                        return Directory.GetDirectories(testDrive);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return null;
                    }
                }

                static IEnumerable<string>? Enumerate(string input)
                {
                    return (input.Length, input.TryGetUpper(0), input.TryGetUpper(1), input.TryGetUpper(2)) switch
                    {
                        (1, >= 'A' and <= 'Z', _, _) => new[] { input + ":\\" }.Concat(EnumerateLogicalDriveOrSubDirectories(input + ":\\") ?? Array.Empty<string>()),
                        (2, >= 'A' and <= 'Z', ':', _) => new[] { input + ":\\" }.Concat(EnumerateLogicalDriveOrSubDirectories(input + "\\") ?? Array.Empty<string>()),
                        (3, >= 'A' and <= 'Z', ':', '\\') => EnumerateLogicalDriveOrSubDirectories(input),
                        _ => Array.Empty<string>()
                    };
                }
            }
        }

        public static IEnumerable<(string path, string? label)> EnumerateLogicalDrives()
        {
            foreach (var driveName in Environment.GetLogicalDrives())
            {
                if (string.IsNullOrEmpty(driveName))
                    continue;

                string? volumeLabel = null;

                try
                {
                    if (new DriveInfo(driveName) is { IsReady: true } driveInfo)
                        volumeLabel = driveInfo.VolumeLabel;
                }
                catch (IOException)
                {
                    // network path not found
                }
                catch (UnauthorizedAccessException)
                {
                    // can't access path
                }

                yield return (driveName, volumeLabel);
            }
        }
    }

    internal static class StringHelper
    {
        public static char TryGetUpper(this string value, int index)
        {
            return char.ToUpper(value.ElementAtOrDefault(index));
        }
    }
}