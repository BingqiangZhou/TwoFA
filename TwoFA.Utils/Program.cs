using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwoFA.Utils.ToolsClass;

namespace TwoFA.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            //Bitmap bitmap = GenerateQRCodeByZxing.GenerateQRCodeToBitmap("hello,world", 128, 128, 1);
            //string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(bitmap);
            //Console.WriteLine(base64String);
            //bitmap.Dispose();

            //SendCodeToEmail.SendCode("1299050656@qq.com", "123456");

            //byte[] bytes = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            //byte[] str = GenerateCode.EncryptAes("123", bytes, bytes);
            //StringBuilder displayString = new StringBuilder();
            //for (int i = 0; i < str.Length; i++)
            //{
            //    displayString.Append(str[i].ToString("X2"));
            //}
            //Console.WriteLine(displayString.ToString());
            //Console.WriteLine(Convert.ToBase64String(str));
            //Console.WriteLine(GenerateCode.DecryptAes(str, bytes, bytes));
            //double counter = Math.Floor(DateTime.Now.ToLocalTime().Ticks / (30 * 1000.0));
            DateTime dt1970 = new DateTime(1970, 1, 1,0,0,0).ToLocalTime();
            DateTime current = DateTime.Now.ToLocalTime();//DateTime.UtcNow for unix timestamp
            TimeSpan span = current - dt1970;
            int counter = (int)Math.Floor(span.TotalMilliseconds/(30*1000.0));
            //Console.WriteLine(span.TotalMilliseconds.ToString());
            var key = "hello";
            //Console.WriteLine(DateTime.Now.Second);
            Console.WriteLine(counter.ToString());
            //double[] codeList = { GenerateCode.GenerateTwoFACode(key,counter-1),
            //GenerateCode.GenerateTwoFACode(key,counter),
            //GenerateCode.GenerateTwoFACode(key,counter+1)};
            //foreach (var item in codeList)
            //{
            //    Console.WriteLine(item);
            //}
            while (true)
            {
                dt1970 = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
                current = DateTime.Now.ToLocalTime();//DateTime.UtcNow for unix timestamp
                span = current - dt1970;
                counter = (int)Math.Floor(span.TotalMilliseconds / (30 * 1000.0));
                Console.WriteLine(GenerateCode.GenerateTwoFACode(key, counter));
                Thread.Sleep(10*1000);
            }
        }
    }
}
