using MatrixYhToolService.MatrixServices;
using MatrixYhToolService.MatrixTool;
using MatrixYhToolService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MatrixYhToolService.Controllers
{
    [ApiController]
    [Route("MatrixApi")]
    public class MatrixYhController : ControllerBase
    {
        private readonly MatrixHandleService _handleService;
        //private readonly ILogger<MatrixYhController> _logger;

        // ILogger<MatrixYhController> logger, 
        public MatrixYhController(MatrixHandleService matrixHandleService)
        {
            //_logger = logger;
            _handleService= matrixHandleService;
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
            //_logger.LogInformation("收到 GetSubmit 请求：request={request}", request.ToString());
            MatrixLogHelper.LogInformation("收到 GetSubmit 请求：request={request}", request.ToString());
            return await _handleService.GetSubmitCall(request);
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
