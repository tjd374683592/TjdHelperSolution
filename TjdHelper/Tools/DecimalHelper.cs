using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    public class DecimalHelper
    {
        public static bool IsHexadecimal(string input)
        {
            // 检查是否以0x或0X开头，并且后续部分是有效的十六进制数字
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                string hexPart = input.Substring(2);
                return int.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out _);
            }
            return false;
        }

        public static bool IsDecimal(string input)
        {
            // 尝试解析为十进制整数
            return int.TryParse(input, out _);
        }
    }
}
