using MatrixYhToolService.MatrixTool;
using MatrixYhToolService.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MatrixYhToolService.Controllers
{
    [ApiController]
    [Route("MatrixApi")]
    public class MatrixYhController : ControllerBase
    {
        private readonly YhInterfaceHelper _yhHelper;

        private readonly ILogger<MatrixYhController> _logger;

        private readonly IWebHostEnvironment _env;//环境
        private readonly IConfiguration _configuration;//路径配置

        public MatrixYhController(ILogger<MatrixYhController> logger, IWebHostEnvironment env, IConfiguration configuration, YhInterfaceHelper yhHelper)
        {
            _logger = logger;
            //_yhHelper = new YhInterfaceHelper();
            _yhHelper = yhHelper;
            _env = env;
            _configuration = configuration;
        }


        /// <summary>
        /// 初始化医保组件接口
        /// </summary>
        /// <returns></returns>
        //[HttpGet("getInitialize", Name = "GetInitialize")]
        //public async Task<MatrixWebResponse> GetInitialize()
        //{
        //    var result = await _yhHelper.InitializeAsync();

        //    return MatrixWebResponse.Success(result);
        //}

        /// <summary>
        /// 注销医保组件
        /// </summary>
        /// <returns></returns>
        //[HttpGet("getDestroy", Name = "GetDestroy")]
        //public async Task<MatrixWebResponse> GetDestroy()
        //{
        //    var result = await _yhHelper.DestroyAsync();

        //    return MatrixWebResponse.Success(result);
        //}


        /// <summary>
        /// 交易接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("getSubmit", Name = "GetSubmit")]
        public async Task<MatrixWebResponse> GetSubmit([FromBody] CallRequestBody request)
        {
            // 使用 ILogger 打印入参
            _logger.LogInformation("收到 GetSubmit 请求：request={request}", request.ToString());

            // 获取内容根目录（需要注入 IWebHostEnvironment）
            string contentRootPath = _env.ContentRootPath;

            // 读取配置中的相对路径
            var call47Path = _configuration["FileStorage:Call47Path"];
            var tempFolderPath = Path.Combine(contentRootPath, MatrixStringTool.checkStr(call47Path, "Call47"));

            // 确保目录存在
            if (!Directory.Exists(tempFolderPath)) Directory.CreateDirectory(tempFolderPath);
         
            // 构建完整的输出文件路径
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string tempFileName = $"{timestamp}.txt";
            string outputFilePath = Path.Combine(tempFolderPath, tempFileName);

            //string tempOrgCode = string.IsNullOrEmpty(request.orgCode) ? "H52263200141" : request.orgCode;
            string tempOrgCode = MatrixStringTool.checkStr(request.orgCode, "H52263200141");

            string tempXmlParameter = $@"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_akb020>{tempOrgCode}</prm_akb020>
                    <prm_begindate>{request.beginDate}</prm_begindate>
                    <prm_enddate>{request.endDate}</prm_enddate>
                    <prm_aac001>{request.pCode}</prm_aac001>
                    <prm_outputfile>{outputFilePath}</prm_outputfile>
                </input>";

            _logger.LogInformation("收到 GetSubmit 请求参数：tempXmlParameter:\n{tempXmlParameter}", tempXmlParameter);

            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);
            if (result.AppCode!=null&& Convert.ToInt32(result.AppCode) > 0)
            {
                if (!System.IO.File.Exists(outputFilePath))
                {
                    _logger.LogInformation($"文件不存在：{outputFilePath}");
                    return MatrixWebResponse.Failure(null,"文件不存在");
                }
                _logger.LogInformation($"文件存在：{outputFilePath}");

                var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath);
                _logger.LogInformation($"读取内容：{parsedData}");

                //return MatrixWebResponse.Success(parsedData);

                if (parsedData != null) // 或者根据具体类型判断，例如 parsedData.Any() 等
                {
                    try
                    {
                        System.IO.File.Delete(outputFilePath);
                        _logger.LogInformation($"文件已删除：{outputFilePath}");

                        return MatrixWebResponse.Success(parsedData);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"删除文件失败：{outputFilePath}");
                        return MatrixWebResponse.Failure(result);
                    }
                }
                else
                {
                    _logger.LogInformation($"文件内容为空，不删除文件：{outputFilePath}");
                    return MatrixWebResponse.Failure("文件内容为空");
                }
            }
            return MatrixWebResponse.Failure(result);
        }

        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet("getTest", Name = "getTest")]
        public async Task<MatrixWebResponse> getTest()
        {
            return MatrixWebResponse.Success("测试Api");
        }
    }
}
