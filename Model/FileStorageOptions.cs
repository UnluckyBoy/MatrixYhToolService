namespace MatrixYhToolService.Model
{
    /// <summary>
    /// 文件配置类
    /// </summary>
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";
        public string UploadPath { get; set; } = string.Empty;
        public string LogPath { get; set; } = string.Empty;// 日志路径
        public string Call47Path { get; set; } = string.Empty;// 47请求文件路径
        public string Call91APath { get; set; } = string.Empty;// 91A请求文件路径
        public string Call45Path { get; set; } = string.Empty;// 45请求文件路径
    }
}
