using MatrixYhToolService.MatrixTool;
using MatrixYhToolService.Model;
using Newtonsoft.Json.Linq;

namespace MatrixYhToolService.MatrixServices
{
    public class MatrixHandleService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly YhInterfaceHelper _yhHelper;
        //private readonly ILogger<MatrixHandleService> _logger;

        private string tempOrgCode = "H52263200141";

        public MatrixHandleService(IWebHostEnvironment env, IConfiguration configuration, YhInterfaceHelper yhHelper)
        {
            _env = env;
            _configuration = configuration;
            _yhHelper = yhHelper;
            //_logger = logger;
        }


        /// <summary>
        /// 公共交易接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MatrixWebResponse> GetSubmitCall(CallRequestBody request)
        {
            MatrixLogHelper.LogInformation($"GetSubmitCall 开始处理请求：{request}");
            //switch (request.callNum?.ToLower())
            switch (request.callNum)
            {
                case "03":
                    return await Submit03Call(request);
                case "42":
                    return await Submit42Call(request);
                case "44":
                    return await Submit44Call(request);
                case "47":
                    return await Submit47Call(request);
                case "H28b":
                    return await SubmitH28bCall(request);
                default:
                    MatrixLogHelper.LogWarning($"未知交易号：{request.callNum}");
                    return MatrixWebResponse.Failure(null, $"不支持的调用类型: {request.callNum}");
            }
        }

        /// <summary>
        /// 03交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> Submit03Call(CallRequestBody request)
        {
            var parameters = new Dictionary<string, string>
            {
                ["prm_payoptype"] = request.prm_payoptype,
                ["businessType"] = request.businessType,
                ["officeId"] = MatrixStringTool.checkStr(request.officeId, "0008"),
                ["officeName"] = MatrixStringTool.checkStr(request.officeName, "收费室"),
                ["operatorId"] = request.operatorId,
                ["operatorName"] = request.operatorName,
                ["orgId"] = MatrixStringTool.checkStr(request.orgCode, tempOrgCode),
                ["deviceType"] = MatrixStringTool.checkStr(request.deviceType,""),
                ["cardtype"] = MatrixStringTool.checkStr(request.cardType, ""),
                ["cardid"] = MatrixStringTool.checkStr(request.cardId, ""),
                ["cardname"] = MatrixStringTool.checkStr(request.cardName, "")
            };

            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");

            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);

            if (result.AppCode != null && Convert.ToInt32(result.AppCode) > 0)
            {
                MatrixLogHelper.LogInformation($"{request.callNum}交易反参_Xml：\n{result.OutputXml}");

                //string tempOuptXml = (string)result.OutputXml;
                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(tempOuptXml);
                //string jsonResult = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
                //// 将 JSON 字符串解析为 JObject
                //JObject jObject = JObject.Parse(jsonResult);
                //// 提取 output 节点（假设根节点名为 output）
                //JObject output = (JObject)jObject["output"];
                //_logger.LogInformation($"返回结果：\n{output}");
                //return MatrixWebResponse.Success(output);

                JObject output = MatrixConvertXmlToJObject.ConvertXmlToJObject((string)result.OutputXml);
                if (output != null)
                {
                    MatrixLogHelper.LogInformation($"返回结果：\n{output}");
                    return MatrixWebResponse.Success(output);
                }
                else
                {
                    return MatrixWebResponse.Failure("XML 解析失败");
                }
            }

            return MatrixWebResponse.Failure(result);
        }


        /// <summary>
        /// 42交易-回退
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> Submit42Call(CallRequestBody request)
        {
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参请求");
            var parameters = new Dictionary<string, string>
            {
                ["regisNum"] = request.regisNum,
                ["clearingCenter"] = request.clearingCenter,
                ["settlementType"] = request.settlementType,
                ["settlementNum"] = request.settlementNum,
                ["operatorId"] = request.operatorId,
                ["operatorName"] = request.operatorName,
                ["opreatorTime"] = request.opreatorTime,
                ["fallbackReasons"] = MatrixStringTool.checkStr(request.fallbackReasons, "门诊回退单边账"),
                ["insuranceMethod"] = request.insuranceMethod,
                ["pCode"] = request.pCode
            };
            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");

            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);

            MatrixLogHelper.LogInformation($"{request.callNum}交易反参：\n{result.OutputXml}");

            if (result.AppCode != null && Convert.ToInt32(result.AppCode) > 0)
            {
                return MatrixWebResponse.Success(result.OutputXml);
            }
            else
            {
                return MatrixWebResponse.Failure(result.OutputXml);
            }

            //return MatrixWebResponse.Failure();
        }

        private async Task<MatrixWebResponse> Submit44Call(CallRequestBody request)
        {
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参请求");
            var parameters = new Dictionary<string, string>
            {
                ["regisNum"] = request.regisNum,
                ["clearingCenter"] = request.clearingCenter,
                ["settlementType"] = request.settlementType,
                ["settlementNum"] = request.settlementNum,
                ["insuranceMethod"] = request.insuranceMethod
            };
            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");

            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);

            MatrixLogHelper.LogInformation($"{request.callNum}交易反参：\n{result.OutputXml}");

            if (result.AppCode != null && Convert.ToInt32(result.AppCode) > 0)
            {
                return MatrixWebResponse.Success(result.OutputXml);
            }
            else
            {
                return MatrixWebResponse.Failure(result.OutputXml);
            }
        }

        /// <summary>
        /// 47交易-医保统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> Submit47Call(CallRequestBody request)
        {
            // 准备文件路径
            string contentRootPath = _env.ContentRootPath;
            var call47Path = _configuration["FileStorage:Call47Path"];
            var tempFolderPath = Path.Combine(contentRootPath, MatrixStringTool.checkStr(call47Path, "Call47"));

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string tempFileName = $"{timestamp}.txt";
            string outputFilePath = Path.Combine(tempFolderPath, tempFileName);

            // 生成 XML 参数
            var parameters = new Dictionary<string, string>
            {
                ["orgCode"] = MatrixStringTool.checkStr(request.orgCode, tempOrgCode),
                ["beginDate"] = request.beginDate,
                ["endDate"] = request.endDate,
                ["pCode"] = request.pCode,
                ["outputFilePath"] = outputFilePath
            };
            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum,parameters);

            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");

            // 调用 COM 组件
            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);

            MatrixLogHelper.LogInformation($"{request.callNum}交易反参：\n{result.OutputXml}");

            // 处理返回结果
            if (result.AppCode != null && Convert.ToInt32(result.AppCode) > 0)
            {
                if (!File.Exists(outputFilePath))
                {
                    MatrixLogHelper.LogWarning($"文件不存在：{outputFilePath}");
                    return MatrixWebResponse.Failure(null, "文件不存在");
                }

                MatrixLogHelper.LogInformation($"解析文件：{outputFilePath}");
                var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath);
                if (parsedData != null)
                {
                    try
                    {
                        File.Delete(outputFilePath);
                        MatrixLogHelper.LogInformation("文件已删除：{FilePath}", outputFilePath);
                        return MatrixWebResponse.Success(parsedData);
                    }
                    catch (Exception ex)
                    {
                        MatrixLogHelper.LogError(ex, "删除文件失败：{FilePath}", outputFilePath);
                        return MatrixWebResponse.Failure(result);
                    }
                }
                else
                {
                    MatrixLogHelper.LogInformation("文件内容为空，不删除文件：{FilePath}", outputFilePath);
                    return MatrixWebResponse.Failure("文件内容为空");
                }
            }

            return MatrixWebResponse.Failure(result);
        }


        /// <summary>
        /// H28b交易-入院时间查询入出院信息(含异地)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> SubmitH28bCall(CallRequestBody request)
        {
            // 准备文件路径
            string contentRootPath = _env.ContentRootPath;
            var call47Path = _configuration["FileStorage:Call47Path"];
            var tempFolderPath = Path.Combine(contentRootPath, MatrixStringTool.checkStr(call47Path, "Call47"));

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string tempFileName = $"{timestamp}.txt";
            string outputFilePath = Path.Combine(tempFolderPath, tempFileName);

            // 生成 XML 参数
            var parameters = new Dictionary<string, string>
            {
                ["orgCode"] = MatrixStringTool.checkStr(request.orgCode, tempOrgCode),
                ["beginDate"] = request.beginDate,
                ["endDate"] = request.endDate,
                ["outputFilePath"] = outputFilePath
            };
            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);

            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");
            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);
            MatrixLogHelper.LogInformation($"{request.callNum}交易反参：\n{result.OutputXml}");
            // 处理返回结果
            if (result.AppCode != null && Convert.ToInt32(result.AppCode) > 0)
            {
                if (!File.Exists(outputFilePath))
                {
                    MatrixLogHelper.LogWarning($"文件不存在：{outputFilePath}");
                    return MatrixWebResponse.Failure(null, "文件不存在");
                }

                MatrixLogHelper.LogInformation($"解析文件：{outputFilePath}");
                var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath);
                if (parsedData != null)
                {
                    try
                    {
                        File.Delete(outputFilePath);
                        MatrixLogHelper.LogInformation("文件已删除：{FilePath}", outputFilePath);
                        return MatrixWebResponse.Success(parsedData);
                    }
                    catch (Exception ex)
                    {
                        MatrixLogHelper.LogError(ex, "删除文件失败：{FilePath}", outputFilePath);
                        return MatrixWebResponse.Failure(result);
                    }
                }
                else
                {
                    MatrixLogHelper.LogInformation("文件内容为空，不删除文件：{FilePath}", outputFilePath);
                    return MatrixWebResponse.Failure("文件内容为空");
                }
            }
            return MatrixWebResponse.Failure(result);
        }
    }
}
