namespace MatrixYhToolService.Model
{
    public class CallRequestBody
    {
        /**47交易参数**/
        public required string callNum { get; set; } //交易编号,必填
        public string? orgCode { get; set; } //机构代码
        public string? beginDate { get; set; } //开始日期
        public string? endDate { get; set; } //结束日期
        public string? pCode { get; set; } //个人编号,选填
        /**47交易参数**/

        /**03交易参数**/
        //支付操作类别,01-药店结算,02-门诊结算,03-住院结算,04-身份识别
        public string? prm_payoptype { get; set; }
        /*用码业务类型:第一编码(01)+第二编码(101-挂号,102-住院建档,103-入院登记,104-缴纳预缴金,201-问诊,202-预约检查,203-检查,204-治疗,301-结算,302-取药,303-取报告,304-打印票据和清单,305-病历材料复印)*/
        public string? businessType { get; set; }
        public string? officeId { get; set; }//医保科室编码
        public string? officeName { get; set; }//医保科室名称
        public string? operatorId { get; set; }//操作员编码
        public string? operatorName { get; set; }//操作员名称
        public string? deviceType { get; set; }//设备类型:自助机该字段设为 SelfService;其它情况不用设置
        public string? cardType { get; set; }//卡类型:
        public string? cardId { get; set; }//身份证ID
        public string? cardName { get; set; }//身份证姓名
        /**03交易参数**/

        /**42参数：个人编码、操作员编码、操作员由上取**/
        public string? regisNum { get; set; }//就诊编号
        public string? clearingCenter { get; set; }//分中心编码
        public string? settlementType { get; set; }//支付类别
        public string? settlementNum { get; set; }//结算编号
        public string? opreatorTime { get; set; }//经办时间
        public string? fallbackReasons { get; set; }//回退原因
        public string? insuranceMethod { get; set; }//执行社会保险办法
        /**42参数**/

        /**H7103参数**/
        public string? omsgId { get; set; }//医保报文ID


        public string? jylsh { get; set; }
        public string? jyyzm { get; set; }

        
    }
}
