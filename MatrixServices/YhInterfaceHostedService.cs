using MatrixYhToolService.MatrixTool;

namespace MatrixYhToolService.MatrixServices
{
    public class YhInterfaceHostedService : IHostedService
    {
        private readonly ILogger<YhInterfaceHostedService> _logger;

        public YhInterfaceHostedService(ILogger<YhInterfaceHostedService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("正在初始化医保组件...");
            var result = await YhInterfaceHelper.Instance.InitializeAsync();
            if (result.IsSuccess)
            {
                _logger.LogInformation("医保组件初始化成功，AppCode: {AppCode}, AppMsg: {AppMsg}", result.AppCode, result.AppMsg);
            }
            else
            {
                _logger.LogError("医保组件初始化失败: {AppMsg}", result.AppMsg);
                // 可根据需要抛出异常阻止应用启动，或继续运行但后续调用会返回错误
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("正在注销医保组件...");
            var result = await YhInterfaceHelper.Instance.DestroyAsync();
            _logger.LogInformation("医保组件注销结果: {AppMsg}", result.AppMsg);
        }
    }
}
