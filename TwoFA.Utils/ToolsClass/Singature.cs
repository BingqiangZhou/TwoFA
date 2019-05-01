using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Utils.ToolsClass
{
    public class Singature
    {
        private static string SHA1(string text)
        {
            string str;
            using (var sha = new SHA1Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                StringBuilder displayString = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    displayString.Append(hash[i].ToString("X2"));
                }
                str = displayString.ToString();
            };
            return str;
        }

        public static string GetUrl(Dictionary<string, string> dict)
        {
            //dict.Add("timestamp", "1555947459");
            ArrayList akeys = new ArrayList(dict.Keys); //别忘了导入System.Collections
            akeys.Sort(); //按字母顺序进行排序
            StringBuilder str = new StringBuilder();//构造URL string
            foreach (string skey in akeys)
            {
                if (str.Length > 0)
                {
                    str.Append("&");
                }
                str.Append(skey);
                str.Append("=");
                str.Append(dict[skey]);
            }
            return str.ToString();
        }

        public static string GetSignature(Dictionary<string, string> dict)
        {
            string url = GetUrl(dict);
            return SHA1(url);
        }
        public static string GetSignature(string urlString)
        {
            return SHA1(urlString);
        }

        public static string GetTimeStamp()
        {
            TimeSpan t = (DateTime.UtcNow.ToLocalTime() - new DateTime(1970, 1, 1));
            int timestamp = (int)t.TotalSeconds;
            return timestamp.ToString();
        }
    }
}
