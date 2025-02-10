using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TjdHelper.Models
{
    // 定义 JSON 数据对应的类
    public class WinErrorInfo
    {
        public int ErrorCode { get; set; }
        public string HexCode { get; set; }
        public string ErrorName { get; set; }
        public string Description { get; set; }
    }
}