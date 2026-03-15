namespace MatrixYhToolService.Model
{
    public class CallRequestBody
    {
        public required string callNum { get; set; } //交易编号
        public required string orgCode { get; set; } //机构代码
        public required string beginDate { get; set; } //开始日期
        public required string endDate { get; set; } //结束日期
        public required string pCode { get; set; } //个人编号,选填
    }
}
