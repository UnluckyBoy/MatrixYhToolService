namespace MatrixYhToolService.MatrixTool
{
    public class MatrixWebResponse
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public Object? content { get; set; }
        //public DateTime timestamp { get; set; }
        public string timestamp { get; set; }
        //public long timestamp { get; set; }

        // 可选的辅助构造函数
        public MatrixWebResponse(bool mSuccess, int mCode, string mMsg, Object? mContent =null)
        {
            this.success = mSuccess;
            this.code = mCode;
            this.msg = mMsg;
            this.content = mContent;
            //this.timestamp = DateTime.Now;
            this.timestamp= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        // 静态快速创建成功响应的方法
        public static MatrixWebResponse Success(Object data, string? msg=null)
        {
            return new MatrixWebResponse(true, ResultCode.SUCCESS.Code,MatrixStringTool.checkStr(msg, ResultCode.SUCCESS.Message), data);
        }

        // 静态快速创建错误响应的方法
        public static MatrixWebResponse Failure()
        {
            return new MatrixWebResponse(false, ResultCode.SERVER_ERROR.Code, ResultCode.SERVER_ERROR.Message);
        }

        public static MatrixWebResponse Failure(Object? data=null, string? msg = null)
        {
            return new MatrixWebResponse(false, ResultCode.SERVER_ERROR.Code, MatrixStringTool.checkStr(msg, ResultCode.SERVER_ERROR.Message), data);
        }
    }
}
