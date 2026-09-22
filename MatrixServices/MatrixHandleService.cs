using MatrixYhToolService.MatrixTool;
using MatrixYhToolService.Model;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;

namespace MatrixYhToolService.MatrixServices
{
    public class MatrixHandleService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly YhInterfaceHelper _yhHelper;
        //private readonly ILogger<MatrixHandleService> _logger;

        // 下载定义数量
        private const int PageSize = 1000;
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
                case "22":
                    return await Submit22Call(request);
                case "42":
                    return await Submit42Call(request);
                //case "44":
                //    return await Submit44Call(request);
                case "45":
                    return await Submit45Call(request);
                case "47":
                    return await Submit47Call(request);
                case "H28b":
                    return await SubmitH28bCall(request);
                case "H7103":
                    return await SubmitH7103Call(request);
                case "H7106":
                    return await SubmitH7106Call(request);
                case "91A":
                    //return await Submit91ACall(request);
                    return await Submit91Call(request);
                case "91B":
                    return await Submit91Call(request);
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
                ["deviceType"] = MatrixStringTool.checkStr(request.deviceType, ""),
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
        /// 22交易接口-入院办理回退
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> Submit22Call(CallRequestBody request)
        {
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参请求");
            var parameters = new Dictionary<string, string>
            {
                ["regisNum"] = request.regisNum,
                ["pCode"] = request.pCode,
                ["settlementType"] = request.settlementType,
                ["clearingCenter"] = request.clearingCenter,
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
        }

        /// <summary>
        /// H7103交易-冲正交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> SubmitH7103Call(CallRequestBody request)
        {
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参请求");
            var parameters = new Dictionary<string, string>
            {
                ["regisNum"] = request.regisNum,
                ["pCode"] = request.pCode,
                ["regisNum"] = request.regisNum,
                ["settlementNum"] = request.settlementNum,
                ["omsgId"] = request.omsgId,
                ["operatorId"] = request.operatorId,
                ["operatorName"] = request.operatorName
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
        /// H7106-异地冲正交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> SubmitH7106Call(CallRequestBody request)
        {
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参请求");
            var parameters = new Dictionary<string, string>
            {
                ["mdtrtarea_admdvs"] = request.mdtrtarea_admdvs,
                ["insuplc_admdvs"] = request.insuplc_admdvs,
                ["sender_msg_id"] = request.omsgId,
                ["settlementType"] = request.settlementType,
                ["pCode"] = request.pCode,
                ["regisNum"] = request.regisNum,
                ["settlementNum"] = request.settlementNum,
                ["operatorId"] = request.operatorId,
                ["operatorName"] = request.operatorName
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
        /// 44交易-医保结算单打印
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        /// 45交易-费用分割
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> Submit45Call(CallRequestBody request)
        {
            string contentRootPath = _env.ContentRootPath;
            var call45Path = _configuration["FileStorage:Call45Path"];
            var tempFolderPath = Path.Combine(contentRootPath, MatrixStringTool.checkStr(call45Path, "Call45"));

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string tempFileName = $"{timestamp}.txt";
            string outputFilePath = Path.Combine(tempFolderPath, tempFileName);

            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参请求");
            var parameters = new Dictionary<string, string>
            {
                ["regisNum"] = request.regisNum,
                ["settlementType"] = request.settlementType,
                ["clearingCenter"] = request.clearingCenter,
                ["outputFilePath"] = outputFilePath
            };
            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);
            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");

            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);
            MatrixLogHelper.LogInformation($"{request.callNum}交易反参：\n{result.OutputXml}");
            if (result.AppCode == null || Convert.ToInt32(result.AppCode) <= 0)
            {
                MatrixLogHelper.LogInformation($"{request.callNum}-交易反参：\n{result.AppMsg}");//写入日志
                return MatrixWebResponse.Failure(result);
            }
            else
            {
                MatrixLogHelper.LogInformation($"{request.callNum}-交易反参：\n{result.OutputXml}");//写入日志

                if (!File.Exists(outputFilePath))
                {
                    MatrixLogHelper.LogWarning($"文件不存在：{outputFilePath}");
                    return MatrixWebResponse.Failure(null, "文件不存在");
                }

                MatrixLogHelper.LogInformation($"解析文件：{outputFilePath}");
                var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath, request.callNum);
                if (parsedData != null)
                {
                    try
                    {
                        File.Delete(outputFilePath);
                        MatrixLogHelper.LogInformation($"文件已删除：{outputFilePath}");
                        return MatrixWebResponse.Success(parsedData);
                    }
                    catch (Exception ex)
                    {
                        MatrixLogHelper.LogError(ex, $"删除文件失败：{outputFilePath}");
                        return MatrixWebResponse.Failure(result);
                    }
                }
                else
                {
                    MatrixLogHelper.LogInformation("文件内容为空，不删除文件：{FilePath}", outputFilePath);
                    return MatrixWebResponse.Failure("文件内容为空");
                }
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
            string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);

            MatrixLogHelper.LogInformation($"生成的{request.callNum}交易入参：\n{tempXmlParameter}");

            // 调用 COM 组件
            var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);
            // 处理返回结果
            if (result.AppCode == null || Convert.ToInt32(result.AppCode) <= 0)
            {
                MatrixLogHelper.LogInformation($"{request.callNum}-交易反参：\n{result.AppMsg}");//写入日志
                return MatrixWebResponse.Failure(result);
            }
            else /*(result.AppCode != null && Convert.ToInt32(result.AppCode) > 0)*/
            {
                MatrixLogHelper.LogInformation($"{request.callNum}-交易反参：\n{result.OutputXml}");//写入日志

                if (!File.Exists(outputFilePath))
                {
                    MatrixLogHelper.LogWarning($"文件不存在：{outputFilePath}");
                    return MatrixWebResponse.Failure(null, "文件不存在");
                }

                MatrixLogHelper.LogInformation($"解析文件：{outputFilePath}");
                //var parsedData = await MatrixCommoFileTool.Read47TxtAsync(outputFilePath);
                var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath, request.callNum);
                if (parsedData != null)
                {
                    try
                    {
                        File.Delete(outputFilePath);
                        MatrixLogHelper.LogInformation($"文件已删除：{outputFilePath}");
                        return MatrixWebResponse.Success(parsedData);
                    }
                    catch (Exception ex)
                    {
                        MatrixLogHelper.LogError(ex, $"删除文件失败：{outputFilePath}");
                        return MatrixWebResponse.Failure(result);
                    }
                }
                else
                {
                    MatrixLogHelper.LogInformation("文件内容为空，不删除文件：{FilePath}", outputFilePath);
                    return MatrixWebResponse.Failure("文件内容为空");
                }
            }
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
                //var parsedData = await MatrixCommoFileTool.ReadH28bTxtAsync(outputFilePath);
                var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath, request.callNum);
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
        /// 91交易接口系列
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Task<MatrixWebResponse> Submit91Call(CallRequestBody request)
        {
            switch (request.callNum)
            {
                case "91A":
                    return Download91CallAsync(request, "Call91A", "91A");

                case "91B":
                    return Download91CallAsync(request, "Call91B", "91B");
                // 以后扩展
                // case "91C":
                //     return Download91CallAsync(request, "Call91C", "91C");

                default:
                    return Task.FromResult(MatrixWebResponse.Failure($"不支持的交易：{request.callNum}"));
            }
        }
        /// <summary>
        /// 91X系列接口公共
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultSubFolder"></param>
        /// <param name="logTag"></param>
        /// <returns></returns>
        private async Task<MatrixWebResponse> Download91CallAsync(CallRequestBody request,string defaultSubFolder,string logTag)
        {
            string contentRootPath = _env.ContentRootPath;
            var call91Path = _configuration["FileStorage:call91Path"];
            var tempFolderPath = Path.Combine(contentRootPath, MatrixStringTool.checkStr(call91Path, defaultSubFolder));
            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            string resultTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            //string resultFilePath = Path.Combine(tempFolderPath, $"result_{request.callNum}_{resultTimestamp}.xlsx
            string resultFilePath = Path.Combine(tempFolderPath, $"result_{request.callNum}.xlsx");

            string downloadNum = request.downloadNum ?? string.Empty;
            int round = 0;
            long totalRows = 0;

            SXSSFWorkbook workbook = null;
            ISheet sheet = null;
            int rowIndex = 0;
            string[] headers = null;

            try
            {
                while (true)
                {
                    round++;
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                    string outputFilePath = Path.Combine(tempFolderPath, $"{timestamp}_{round}.txt");

                    var parameters = new Dictionary<string, string>
                    {
                        ["downloadNum"] = downloadNum,
                        ["outputFilePath"] = outputFilePath
                    };
                    string tempXmlParameter = MatrixXmlTemplate.GenerateXml(request.callNum, parameters);
                    MatrixLogHelper.LogInformation($"[{request.callNum}] 第{round}次入参（downloadNum={downloadNum}）");

                    var result = await _yhHelper.CallAsync(request.callNum, tempXmlParameter);

                    if (result.AppCode == null || Convert.ToInt32(result.AppCode) <= 0)
                    {
                        MatrixLogHelper.LogInformation($"[{request.callNum}] 第{round}次反参：\n{result.AppMsg}");
                        SaveWorkbook(workbook, resultFilePath);
                        return MatrixWebResponse.Failure(result);
                    }

                    MatrixLogHelper.LogInformation($"[{request.callNum}] 第{round}次反参：\n{result.OutputXml}");

                    if (!File.Exists(outputFilePath))
                    {
                        MatrixLogHelper.LogWarning($"文件不存在：{outputFilePath}");
                        SaveWorkbook(workbook, resultFilePath);
                        return MatrixWebResponse.Failure(null, "文件不存在");
                    }

                    var parsedData = await MatrixCommoFileTool.ReadTxtAsync(outputFilePath, request.callNum);
                    int batchCount = parsedData?.Count ?? 0;

                    if (batchCount == 0)
                    {
                        MatrixLogHelper.LogInformation($"第{round}次返回为空，下载结束");
                        TryDeleteFile(outputFilePath);
                        break;
                    }

                    if (workbook == null)
                    {
                        workbook = new SXSSFWorkbook(100) { CompressTempFiles = true };
                        sheet = workbook.CreateSheet("result");
                    }

                    if (headers == null)
                    {
                        headers = parsedData[0].Keys.ToArray();
                        var headerRow = sheet.CreateRow(rowIndex++);
                        for (int i = 0; i < headers.Length; i++)
                            headerRow.CreateCell(i).SetCellValue(headers[i]);
                    }

                    foreach (var row in parsedData)
                    {
                        var dataRow = sheet.CreateRow(rowIndex++);
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var val = row.TryGetValue(headers[i], out var v) ? v : null;
                            dataRow.CreateCell(i).SetCellValue(val ?? string.Empty);
                        }
                    }
                    totalRows += batchCount;

                    string maxLsh = GetMaxLsh(parsedData);

                    parsedData = null;
                    TryDeleteFile(outputFilePath);

                    if (batchCount < PageSize)
                    {
                        MatrixLogHelper.LogInformation($"第{round}次返回 {batchCount} 行（< {PageSize}），下载完成");
                        break;
                    }

                    if (string.IsNullOrEmpty(maxLsh))
                    {
                        MatrixLogHelper.LogWarning("未取到 hilistLsh，终止下载防止死循环");
                        break;
                    }
                    if (string.Equals(maxLsh, downloadNum, StringComparison.Ordinal))
                    {
                        MatrixLogHelper.LogWarning($"流水号未推进（仍为 {downloadNum}），终止下载防止死循环");
                        break;
                    }

                    downloadNum = maxLsh;
                }
            }
            catch (Exception ex)
            {
                MatrixLogHelper.LogError(ex, $"{logTag} 下载出错（已下载 {totalRows} 行）");
                SaveWorkbook(workbook, resultFilePath);
                return MatrixWebResponse.Failure($"下载出错：{ex.Message}，已保存：{resultFilePath}");
            }

            SaveWorkbook(workbook, resultFilePath);

            MatrixLogHelper.LogInformation($"{logTag} 下载完成，共 {totalRows} 行，文件：{resultFilePath}");

            return MatrixWebResponse.Success(new
            {
                filePath = resultFilePath,
                totalRows,
                rounds = round
            });
        }

        private void SaveWorkbook(SXSSFWorkbook workbook, string path)
        {
            if (workbook == null) return;
            try
            {
                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Flush();
                MatrixLogHelper.LogInformation($"xlsx 已保存：{path}");
            }
            catch (Exception ex)
            {
                MatrixLogHelper.LogError(ex, $"保存 xlsx 失败：{path}");
            }
            finally
            {
                try { workbook.Close(); } catch { }
                try { workbook.Dispose(); } catch { }
            }
        }

        /// <summary>
        /// 取本次数据中 hilistLsh 的最大值（优先按数值比较，非数值按字符串比较兜底）
        /// </summary>
        private static string GetMaxLsh(List<Dictionary<string, string>> data)
        {
            long maxNum = long.MinValue;
            string maxStr = null;

            foreach (var row in data)
            {
                if (!row.TryGetValue("hilistLsh", out var lsh) || string.IsNullOrWhiteSpace(lsh))
                    continue;

                if (long.TryParse(lsh, out var num))
                {
                    if (num > maxNum)
                    {
                        maxNum = num;
                        maxStr = lsh;
                    }
                }
                else
                {
                    // 非数字流水号，按字符串比较兜底
                    if (maxStr == null || string.Compare(lsh, maxStr, StringComparison.Ordinal) > 0)
                        maxStr = lsh;
                }
            }

            return maxStr;
        }

        /// <summary>
        /// 安全删除文件（不抛出异常）
        /// </summary>
        private void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    MatrixLogHelper.LogInformation($"文件已删除：{path}");
                }
            }
            catch (Exception ex)
            {
                MatrixLogHelper.LogError(ex, $"删除文件失败：{path}");
            }
        }
      
    }
}
