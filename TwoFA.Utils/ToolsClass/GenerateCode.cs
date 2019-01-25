using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Utils.ToolsClass
{
    public class GenerateCode
    {
        public static int GenerateEmailCode(int minValue,int maxValue)
        {
            return new Random().Next(minValue, maxValue);
        }
    }
}
