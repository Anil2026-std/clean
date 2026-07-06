namespace ImageServer.dto
{
    public class MediaSettings
    {
        public int MaxFileSizeMB { get; set; }
        public List<string> AllowedFileExtensions { get; set; } = new List<string>();
    }
}
