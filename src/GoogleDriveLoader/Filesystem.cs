using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

namespace GoogleDriveLoader
{
    internal static class Filesystem
    {
        private static string rootDirectory;

        internal static void Initialize(string root)
        {
            rootDirectory = root;
            EnsureRootDirectoryExists();
        }       

        internal static FileStream CreateFile(string name, string directoryPath)
        {
            var path = Path.Combine(directoryPath, name);

            return new FileStream(path, FileMode.Create);
        }

        internal static string CreateDirectory(string name)
        {
            var path = Path.Combine(rootDirectory, name);
            Directory.CreateDirectory(path);

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            return path;
        }

        internal static IEnumerable<string> GetFileHashes(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return Enumerable.Empty<string>();

            var hashes = new List<string>();

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                using var md5 = MD5.Create();
                using var stream = File.OpenRead(file);

                var hash = md5.ComputeHash(stream);
                hashes.Add(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
            }

            return hashes;
        }
        
        private static void EnsureRootDirectoryExists()
        {
            Directory.CreateDirectory(rootDirectory);

            if (!Directory.Exists(rootDirectory))
                throw new DirectoryNotFoundException(nameof(rootDirectory));
        }
    }
}
