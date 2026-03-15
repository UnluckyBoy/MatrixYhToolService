using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text;

namespace MatrixYhToolService.MatrixTool
{
    /// <summary>
    /// 文件公共方法
    /// </summary>
    public class MatrixCommoFileTool
    {
        // 预定义47交易列表
        private static readonly string[] call47ColumnKeys = new[]
        {
            "pCode", "pPayAmount", "costTotal", "selfAmount", "payHookAmount", "rangeAmount",
            "deductibleAmount", "medicalPayAmount", "bigMedicalPayAmount", "civilServantAmount",
            "pAccountBalance", "clearingCenter", "clearingType", "clearingMode", "clearingNum",
            "medicalPersonType", "payType", "personType", "personIdentity", "opreatorTime",
            "regisNum", "pName", "settlementNum", "insuranceMethod", "settlementType",
            "administrativeDivision", "opreatorCode", "opreatorName", "medicalAssistance",
            "hmCompensation", "consoleCompensation", "otherFunds", "medicalVisitType",
            "diseaseCode", "diseaseName", "insuranceCode", "pIdNum", "specialType",
            "settlementMsgId", "insuranceWalletPayAmount"
        };

        /// <summary>
        /// 47交易文件处理
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<List<Dictionary<string, string>>> ReadTxtAsync(string filePath)
        {
            // 读取文件内容并解析（使用 GBK 编码）
            var lines = await System.IO.File.ReadAllLinesAsync(filePath, Encoding.GetEncoding("GBK"));

            var parsedData = new List<Dictionary<string, string>>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var columns = line.Split('\t', StringSplitOptions.None); // 按制表符分割，保留空列
                var rowDict = new Dictionary<string, string>();

                // 自动生成列名：Column1, Column2, ...
                for (int i = 0; i < columns.Length; i++)
                {
                    //string key = $"Column{i + 1}";
                    string key;
                    if (i < call47ColumnKeys.Length)
                    {
                        key = call47ColumnKeys[i];
                    }
                    else
                    {
                        key = $"Column{i + 1}";
                    }
                    rowDict[key] = columns[i];
                }

                parsedData.Add(rowDict);
            }

            return parsedData;
        }
    }
}
