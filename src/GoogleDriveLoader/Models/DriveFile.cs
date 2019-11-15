using GoogleDriveLoader.Helpers;

namespace GoogleDriveLoader.Models
{
    public class DriveFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public string Checksum { get; set; }

        public DriveFile(string id, string name, long? size, string checksum)
        {
            Id = id;
            Name = name;
            Size = FileSizeCalculator.ToMegabyte(size);
            Checksum = checksum;
        }
    }
}
