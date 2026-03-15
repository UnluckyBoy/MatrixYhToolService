namespace MatrixYhToolService.MatrixTool
{
    public class YhInterfaceResult
    {
        /// <summary>
        /// 业务返回码（COM 方法填充的 Appcode）。通常 >0 表示成功，<0 表示失败。
        /// </summary>
        public object? AppCode { get; set; }

        /// <summary>
        /// 业务返回消息（COM 方法填充的 Appmsg）。
        /// </summary>
        public object? AppMsg { get; set; }

        /// <summary>
        /// 业务流水号（仅 Call 方法有值）
        /// </summary>
        public object? BusinessSequence { get; set; }

        /// <summary>
        /// 业务验证信息（仅 Call 方法有值）
        /// </summary>
        public object? BusinessValidate { get; set; }

        /// <summary>
        /// 输出 XML（仅 Call 方法有值）
        /// </summary>
        public object? OutputXml { get; set; }

        /// <summary>
        /// 非业务异常信息（如 COM 调用异常、参数校验失败等）。若此属性不为空，表示发生了未到达业务层的错误。
        /// </summary>
        public string? ErrorMessage { get; set; }

        // 判断是否成功的属性
        public bool IsSuccess
        {
            get
            {
                if (AppCode == null)
                    return false;

                try
                {
                    // 使用 Convert.ToInt32 处理多种类型（如 int, string, null 等）
                    int code = Convert.ToInt32(AppCode);
                    return code > 0;
                }
                catch
                {
                    // 转换失败（如非数字字符串）视为失败
                    return false;
                }
            }
        }

        /// <summary>
        /// 创建成功结果（业务调用成功，但业务本身可能返回错误码，需由调用方检查 AppCode）
        /// </summary>
        public static YhInterfaceResult Success(object appCode, object appMsg,
            object? businessSequence = null, object? businessValidate = null, object? outputXml = null)
        {
            return new YhInterfaceResult
            {
                AppCode = appCode,
                AppMsg = appMsg,
                BusinessSequence = businessSequence,
                BusinessValidate = businessValidate,
                OutputXml = outputXml,
                ErrorMessage = null // 明确设为 null
            };
        }

        /// <summary>
        /// 创建失败结果（用于非业务异常）
        /// </summary>
        public static YhInterfaceResult Fail(string errorMessage)
        {
            return new YhInterfaceResult
            {
                AppCode = 0,
                AppMsg = "调用接口异常",
                ErrorMessage = errorMessage
            };
        }
    }
}
