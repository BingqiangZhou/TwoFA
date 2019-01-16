using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace TwoFA.Utils.ToolsClass
{
    //https://blog.csdn.net/xwnxwn/article/details/72636417
    class GenerateQRCodeByZxing
    {
        //生成二维码并返回System.Drawing.Bitmap对象
        public static Bitmap GenerateQRCodeToBitmap(string text,int height,int width,int margin)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE//修改形式，可生成条形码等
            };
            QrCodeEncodingOptions options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                //设置内容编码
                CharacterSet = "UTF-8",
                //设置二维码的宽度和高度
                Width = width,
                Height = height,
                //设置二维码的边距,单位不是固定像素
                Margin = margin
            };
            writer.Options = options;

            Bitmap bitmap = writer.Write(text);
            //string filename = @"H:\桌面\截图\generate1.png";
            //bitmap.Save(filename, ImageFormat.Png);
            //bitmap.Dispose();
            return bitmap;
        }

        //生产二维码并保存为文件
        public static void GenerateQRCodeToFile(string text, int height, int width, int margin,string toFilePath,ImageFormat imageFormat)
        {
            Bitmap bitmap = GenerateQRCodeToBitmap(text, height, width, margin);
            bitmap.Save(toFilePath, imageFormat);
            bitmap.Dispose();

        }
    }
}
