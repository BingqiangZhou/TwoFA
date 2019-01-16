using System;
using System.Collections.Generic;
using System.Drawing;//手动添加引用
using System.Drawing.Imaging;//手动添加引用
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Utils.ToolsClass
{
    //https://blog.csdn.net/weixin_43260645/article/details/82812127
    class BitmapAndBase64MutualTransformation
    {
        //图片转为base64编码的字符串 
        public static string BitmapToBase64String(Bitmap bmp)
        {
            try {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        //base64编码的字符串转为图片 
        public static Bitmap Base64StringToBitmap(string strbase64)
        {
            try {
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                return bmp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
