namespace GoogleDriveLoader.Helpers
{
    internal static class FileSizeCalculator
    {
        internal static int ToMegabyte(long? size)
        {
            return size == null ? 0 : (int)size.Value / 1024 / 1024;
        }
    }
}
