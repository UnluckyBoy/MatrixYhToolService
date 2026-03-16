using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace MatrixYhToolService.MatrixTool
{
    /// <summary>
    /// Xml-Json互转
    /// </summary>
    public class MatrixConvertXmlToJObject
    {
        /// <summary>
        /// 将 XML 字符串转换为 JObject，并提取指定的根节点（默认为 "output"）
        /// </summary>
        /// <param name="xml">XML 字符串</param>
        /// <param name="rootNodeName">要提取的根节点名称，默认为 "output"</param>
        /// <returns>提取的 JObject，如果节点不存在或 XML 无效则返回 null</returns>
        public static JObject ConvertXmlToJObject(string xml, string rootNodeName = "output")
        {
            if (string.IsNullOrWhiteSpace(xml))
                return null;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                // 将整个 XML 文档序列化为 JSON 字符串
                string json = JsonConvert.SerializeXmlNode(doc);

                // 解析为 JObject
                JObject jObject = JObject.Parse(json);

                // 返回指定的根节点（默认 output）
                return jObject[rootNodeName] as JObject;
            }
            catch (Exception ex)
            {
                // 可根据需要记录异常日志
                MatrixLogHelper.LogError($"返回结果：\n{ex.Message}");
                return null;
            }
        }
    }
}
