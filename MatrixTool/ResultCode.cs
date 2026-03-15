namespace MatrixYhToolService.MatrixTool
{
    public class ResultCode
    {
        public int Code { get; }
        public string Message { get; }

        private ResultCode(int code, string message)
        {
            Code = code;
            Message = message;
        }

        // 预定义返回码（对应你提供的枚举）
        public static readonly ResultCode SUCCESS = new(200, "请求成功!");
        public static readonly ResultCode REPEAT_ERROR = new(402, "重数据重复!");
        public static readonly ResultCode FAILED = new(404, "请求失败!");
        public static readonly ResultCode NO_LOGIN = new(405, "未登录");
        public static readonly ResultCode SERVER_ERROR = new(500, "服务器异常!");
        public static readonly ResultCode TOKEN_ERROR = new(501, "令牌错误");
        public static readonly ResultCode TOKEN_EXPIRED = new(502, "令牌过期");
        public static readonly ResultCode QUERY_ERROR = new(504, "查询异常!");
        public static readonly ResultCode PARAM_ERROR = new(505, "请求参数异常!");
        public static readonly ResultCode ZERO_RESULT = new(999, "0数据结果");

        // 允许通过 code 查找对应的 ResultCode
        private static readonly Dictionary<int, ResultCode> _codeMap = new()
        {
            { SUCCESS.Code, SUCCESS },
            { REPEAT_ERROR.Code, REPEAT_ERROR },
            { SERVER_ERROR.Code, SERVER_ERROR },
            { FAILED.Code, FAILED },
            { NO_LOGIN.Code, NO_LOGIN },
            { TOKEN_ERROR.Code, TOKEN_ERROR },
            { TOKEN_EXPIRED.Code, TOKEN_EXPIRED },
            { QUERY_ERROR.Code, QUERY_ERROR },
            { PARAM_ERROR.Code, PARAM_ERROR },
            { ZERO_RESULT.Code, ZERO_RESULT }
        };

        public static ResultCode FromCode(int code) =>_codeMap.TryGetValue(code, out var result) ? result : throw new ArgumentException($"未知返回码: {code}");
    }
}
