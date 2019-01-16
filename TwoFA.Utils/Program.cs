using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.Utils.ToolsClass;

namespace TwoFA.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bitmap = GenerateQRCodeByZxing.GenerateQRCodeToBitmap("hello,world", 128, 128, 1);
            string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(bitmap);
            Console.WriteLine(base64String);
            bitmap.Dispose();
        }
    }
}
