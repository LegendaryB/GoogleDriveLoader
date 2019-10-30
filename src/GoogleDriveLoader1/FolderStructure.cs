using System;
using System.IO;

namespace GoogleDriveLoader
{
    internal class FolderStructure
    {
        private static FolderStructure _instance;       
        
        internal static FolderStructure Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FolderStructure();

                return _instance;
            }
        }

        private readonly string _outputFolder;

        private FolderStructure() { }

        internal void SetOutputFolder(string outputFolder)
        {
            if (string.IsNullOrWhiteSpace(outputFolder))
                throw new ArgumentException(nameof(outputFolder));

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);
        }

        internal void MakeMediaFolder(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException(nameof(title));

            Directory.CreateDirectory(Path.Combine(_outputFolder, title));
        }
    }
}
