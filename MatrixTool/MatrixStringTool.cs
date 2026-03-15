namespace MatrixYhToolService.MatrixTool
{
    public class MatrixStringTool
    {

        /// <summary>
        /// 判断字符串并返回值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="mDefualt"></param>
        /// <returns></returns>
        public static string checkStr(string str,string mDefualt)
        {
            return string.IsNullOrEmpty(str) ? mDefualt : str;
        }
    }
}
