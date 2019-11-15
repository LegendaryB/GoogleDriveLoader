using System.Collections.Generic;
using System.Linq;

namespace GoogleDriveLoader.Models
{
    public class DriveDirectory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public IList<DriveFile> Files { get; set; }

        public bool ContainsFiles => Files?.Any() == true;
    }
}
