using yh_interfaceproxy;
using System.Runtime.ExceptionServices;

namespace MatrixYhToolService.MatrixTool
{
    public class YhInterfaceHelper : IDisposable
    {
        private readonly CoClass_yhinterface _comInstance;
        private readonly object _lock = new object();
        private bool _disposed;
        private bool _initialized;
        private YhInterfaceResult _initResult; // 保存初始化结果

        // 私有构造函数
        private YhInterfaceHelper()
        {
            _comInstance = new CoClass_yhinterface();
        }

        // 单例实例
        private static readonly Lazy<YhInterfaceHelper> _instance = new Lazy<YhInterfaceHelper>(() => new YhInterfaceHelper());
        public static YhInterfaceHelper Instance => _instance.Value;

        /// <summary>
        /// 初始化方法（被调用一次，由后台服务触发）
        /// </summary>
        /// <returns></returns>
        public async Task<YhInterfaceResult> InitializeAsync()
        {
            if (_initialized) return _initResult;

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    if (_initialized) return _initResult; // 双重检查锁定

                    try
                    {
                        object appCode = new object();
                        object appMsg = new object();
                        _comInstance.yh_interface_init(ref appCode, ref appMsg);
                        _initialized = true;
                        _initResult = YhInterfaceResult.Success(appCode, appMsg);
                    }
                    catch (Exception ex)
                    {
                        _initResult = YhInterfaceResult.Fail($"初始化失败: {ex.Message}");
                    }
                    return _initResult;
                }
            });
        }

        /// <summary>
        /// 销毁方法（应用停止时调用）
        /// </summary>
        /// <returns></returns>
        public async Task<YhInterfaceResult> DestroyAsync()
        {
            if (!_initialized) return YhInterfaceResult.Success(1, "Not initialized");

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        _comInstance.yh_interface_destroy();
                        _initialized = false;
                        return YhInterfaceResult.Success(1, "组件已注销");
                    }
                    catch (Exception ex)
                    {
                        return YhInterfaceResult.Fail($"注销失败: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// 调用方法，确保已初始化
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="dataXml"></param>
        /// <returns></returns>
        public async Task<YhInterfaceResult> CallAsync(string businessId, string dataXml)
        {
            if (string.IsNullOrWhiteSpace(dataXml))
                return YhInterfaceResult.Fail("XML 参数不能为空");

            // 如果未初始化，返回错误（或者你也可以在这里自动初始化，但更推荐在应用启动时确保初始化）
            if (!_initialized)
            {
                return YhInterfaceResult.Fail("组件尚未初始化，请先调用 InitializeAsync");
            }

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        object businessSequence = new object();
                        object businessValidate = new object();
                        object outputXml = new object();
                        object appCode = new object();
                        object appMsg = new object();

                        _comInstance.yh_interface_call(
                            businessId,
                            dataXml,
                            ref businessSequence,
                            ref businessValidate,
                            ref outputXml,
                            ref appCode,
                            ref appMsg);

                        return YhInterfaceResult.Success(appCode, appMsg, businessSequence, businessValidate, outputXml);
                    }
                    catch (Exception ex)
                    {
                        return YhInterfaceResult.Fail($"调用失败: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// 确认操作
        /// </summary>
        /// <param name="jylsh"></param>
        /// <param name="jyyzm"></param>
        /// <returns></returns>
        public async Task<YhInterfaceResult> ConfirmAsync(string jylsh, string jyyzm)
        {
            if (!_initialized)
                return YhInterfaceResult.Fail("组件尚未初始化，请先调用 InitializeAsync");

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        object appCode = new object();
                        object appMsg = new object();

                        _comInstance.yh_interface_confirm(jylsh, jyyzm, ref appCode, ref appMsg);
                        return YhInterfaceResult.Success(appCode, appMsg);
                    }
                    catch (Exception ex)
                    {
                        return YhInterfaceResult.Fail($"确认失败: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="jylsh"></param>
        /// <returns></returns>
        public async Task<YhInterfaceResult> CancelAsync(string jylsh)
        {
            if (!_initialized)
                return YhInterfaceResult.Fail("组件尚未初始化，请先调用 InitializeAsync");

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        object appCode = new object();
                        object appMsg = new object();

                        _comInstance.yh_interface_cancel(jylsh, ref appCode, ref appMsg);
                        return YhInterfaceResult.Success(appCode, appMsg);
                    }
                    catch (Exception ex)
                    {
                        return YhInterfaceResult.Fail($"取消失败: {ex.Message}");
                    }
                }
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // 可在此释放 COM 资源
                _disposed = true;
            }
        }
    }
}