namespace MatrixYhToolService.Model
{
    /// <summary>
    /// 文件配置类
    /// </summary>
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";
        public string UploadPath { get; set; } = string.Empty;
        public string LogPath { get; set; } = string.Empty;
        public string Call47Path { get; set; } = string.Empty;
    }
}
