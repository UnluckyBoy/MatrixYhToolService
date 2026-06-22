namespace MatrixYhToolService.MatrixTool
{
    public class MatrixXmlTemplate
    {
        private static readonly Dictionary<string, string> XmlTemplates = new Dictionary<string, string>
        {
            ["47"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_akb020>{orgCode}</prm_akb020>
                    <prm_begindate>{beginDate}</prm_begindate>
                    <prm_enddate>{endDate}</prm_enddate>
                    <prm_aac001>{pCode}</prm_aac001>
                    <prm_outputfile>{outputFilePath}</prm_outputfile>
                </input>",
            ["H28b"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_begindate>{beginDate}</prm_begindate>
                    <prm_enddate>{endDate}</prm_enddate>
                    <prm_akb020>{orgCode}</prm_akb020>
                    <prm_outputfile>{outputFilePath}</prm_outputfile>
                </input>",
            ["03"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_payoptype>{prm_payoptype}</prm_payoptype>
                    <businessType>{businessType}</businessType>
                    <officeId>{officeId}</officeId>
                    <officeName>{officeName}</officeName>
                    <operatorId>{operatorId}</operatorId>
                    <operatorName>{operatorName}</operatorName>
                    <orgId>{orgId}</orgId>
                    <deviceType>{deviceType}</deviceType>
                    <cardtype>{cardtype}</cardtype>
                    <cardid>{cardid}</cardid>
                    <cardname>{cardname}</cardname>
                </input>",
            ["22"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_akc190>{regisNum}</prm_akc190>
                    <prm_aac001>{pCode}</prm_aac001>
                    <prm_aka130>{settlementType}</prm_aka130>
                    <prm_yab003>{clearingCenter}</prm_yab003>
                    <prm_ykb065>{insuranceMethod}</prm_ykb065>
                </input>",
            ["42"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_akc190>{regisNum}</prm_akc190>
                    <prm_yab003>{clearingCenter}</prm_yab003>
                    <prm_aka130>{settlementType}</prm_aka130>
                    <prm_yka103>{settlementNum}</prm_yka103>
                    <prm_aae011>{operatorId}</prm_aae011>
                    <prm_ykc141>{operatorName}</prm_ykc141>
                    <prm_aae036>{opreatorTime}</prm_aae036>
                    <prm_aae013>{fallbackReasons}</prm_aae013>
                    <prm_ykb065>{insuranceMethod}</prm_ykb065>
                    <prm_aac001>{pCode}</prm_aac001>
                </input>",
            ["H7103"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_aac001>{pCode}</prm_aac001>
                    <prm_akc190>{regisNum}</prm_akc190>
                    <prm_yka103>{settlementNum}</prm_yka103>
                    <omsgid>{omsgId}</omsgid>
                </input>",
            ["H7106"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <mdtrtarea_admdvs>{mdtrtarea_admdvs}</mdtrtarea_admdvs>
                    <insuplc_admdvs>{insuplc_admdvs}</insuplc_admdvs>
                    <sender_msg_id>{sender_msg_id}</sender_msg_id>
                    <aka130>{settlementType}</aka130>
                    <aac001>{pCode}</aac001>
                    <akc190>{regisNum}</akc190>
                    <yka103>{settlementNum}</yka103>
                    <prm_yabtch></prm_yabtch>
                </input>",
            ["44"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <prm_akc190>{regisNum}</prm_akc190>
                    <prm_yab003>{clearingCenter}</prm_yab003>
                    <prm_aka130>{settlementType}</prm_aka130>
                    <prm_akc021></prm_akc021>
                    <prm_yka103>{settlementNum}</prm_yka103>
                    <prm_ykb065>{insuranceMethod}</prm_ykb065>
                </input>",
            ["OTHER_ORG_CODE"] = @"<?xml version=""1.0"" encoding=""GBK"" standalone=""yes"" ?>
                <input>
                    <field1>{orgCode}</field1>
                    <start_date>{beginDate}</start_date>
                    <end_date>{endDate}</end_date>
                    <patient_id>{pCode}</patient_id>
                    <outfile>{outputFilePath}</outfile>
                </input>",
        };

        private const string DefaultTemplateKey = "47";//默认交易号

        /// <summary>
        /// 根据机构编码获取对应的 XML 模板字符串（未经替换）
        /// </summary>
        /// <param name="callNum">交易编号</param>
        /// <returns>XML 模板，若未找到则返回默认模板</returns>
        public static string GetTemplate(string callNum)
        {
            if (string.IsNullOrEmpty(callNum))
                callNum = DefaultTemplateKey;

            if (XmlTemplates.TryGetValue(callNum, out var template))
                return template;

            //MatrixLogHelper.LogWarning($"Xml模板匹配：{XmlTemplates[DefaultTemplateKey]}");//打印日志
            return XmlTemplates[DefaultTemplateKey];
        }

        /// <summary>
        /// 生成最终的 XML 参数字符串（使用参数字典替换占位符）
        /// </summary>
        /// <param name="callNum">交易编号</param>
        /// <param name="parameters">包含占位符名称与值的字典</param>
        /// <returns>填充后的 XML 字符串</returns>
        public static string GenerateXml(string callNum, IDictionary<string, string> parameters)
        {
            var template = GetTemplate(callNum);
            if (parameters == null || parameters.Count == 0)
                return template;

            // 遍历字典，将 {key} 替换为对应的值
            foreach (var kvp in parameters)
            {
                // 避免 null 值，用空字符串代替
                string value = kvp.Value ?? "";
                template = template.Replace("{" + kvp.Key + "}", value);
            }

            return template;
        }

        /// <summary>
        /// 检查是否存在指定机构编码的模板
        /// </summary>
        public static bool HasTemplate(string orgCode) => !string.IsNullOrEmpty(orgCode) && XmlTemplates.ContainsKey(orgCode);
    }
}
