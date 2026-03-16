namespace MatrixYhToolService.MatrixTool
{
    /// <summary>
    /// 日志公共类
    /// </summary>
    public class MatrixLogHelper
    {
        private static ILoggerFactory _loggerFactory;
        private static ILogger _logger;

        /// <summary>
        /// 初始化日志工厂（在应用启动时调用）
        /// </summary>
        public static void Initialize(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            // 使用一个通用的类别名称，或者根据需要创建不同类别的 logger
            _logger = _loggerFactory.CreateLogger("Global");
        }

        // 也可以提供创建特定类别 logger 的方法
        public static ILogger CreateLogger<T>()
        {
            if (_loggerFactory == null)
                throw new InvalidOperationException("LogHelper 尚未初始化，请先在启动时调用 Initialize 方法。");
            return _loggerFactory.CreateLogger<T>();
        }

        public static ILogger CreateLogger(string categoryName)
        {
            if (_loggerFactory == null)
                throw new InvalidOperationException("LogHelper 尚未初始化，请先在启动时调用 Initialize 方法。");
            return _loggerFactory.CreateLogger(categoryName);
        }

        // 以下是常用的日志方法（直接使用全局 logger）
        public static void LogInformation(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }

        public static void LogError(string message, params object[] args)
        {
            _logger?.LogError(message, args);
        }

        public static void LogError(Exception ex, string message, params object[] args)
        {
            _logger?.LogError(ex, message, args);
        }

        public static void LogWarning(string message, params object[] args)
        {
            _logger?.LogWarning(message, args);
        }
    }
}
