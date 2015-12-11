using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace GetWebHtml
{

    class Program
    {

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
            );

        public static readonly string _path = "d:\\ouput.html";
        public static int _width = 1920, _height = 1080;

        static string GetKeyWord()
        {
            if (!File.Exists("KeyWord"))
                File.Create("KeyWord").Close();

            StreamReader sr = new StreamReader("KeyWord");

            string str = "";
            string data = "";
            while ((str = sr.ReadLine()) != null)
            {
                data += str + "\n";
            }

            if (data == "")
            {
                Console.WriteLine("没有关键词");
                Environment.Exit(0);
            }
            var datas = data.Split('\n');
            sr.Close();
            return datas[new Random().Next(0, datas.Length - 1)];
        }

        static string[] GetPicURl(string strs)
        {
            var datas = strs.Split(new string[] { "\"objURL\":" }, System.StringSplitOptions.RemoveEmptyEntries);

            int start = 0, end = 0;
            for (int i = 1; i < datas.Length; i++)
            {
                start = datas[i].IndexOf("\"") + "\"".Length;
                end = datas[i].IndexOf("\",");
                datas[i] = datas[i].Substring(start, end - start);
            }
            return datas;
        }

        static void Main(string[] args)
        {
            if (!File.Exists("IMGResolution"))
                File.Create("IMGResolution").Close();
            StreamReader sr = new StreamReader("IMGResolution");
            string msg = sr.ReadLine();
            if (msg.Split(',').Length == 2)
            {
                _width = Int32.Parse(msg.Split(',')[0]);
                _height = Int32.Parse(msg.Split(',')[1]);
            }
            sr.Close();

            try
            {

                WebClient MyWebClient = new WebClient();


                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据

                Byte[] pageData = MyWebClient.DownloadData("http://image.baidu.com/search/index?ct=&z=&tn=baiduimage&ipn=r&word=" + GetKeyWord() + "&pn=0&ie=utf-8&oe=utf-8&cl=&lm=-1&fr=&se=&sme=&width=" + _width + "&height=" + _height); //从指定网站下载数据

                // string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            

                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句

                //Console.WriteLine(pageHtml);//在控制台输入获取的内容

                /*
                if (!File.Exists(_path))
                    File.Create(_path);

                
                using (StreamWriter sw = new StreamWriter(_path, false))//将获取的内容写入文本
                {

                    sw.Write(pageHtml);
                    sw.Close();

                }
                */

                if (!Directory.Exists("C:\\Users\\xiaoke\\Pictures\\ダウンロード　ピクチャ\\"))
                    Directory.CreateDirectory("C:\\Users\\xiaoke\\Pictures\\ダウンロード　ピクチャ\\");
                while (true)
                {
                    var strs = GetPicURl(pageHtml);
                    Random ram = new Random();
                    string URL = strs[ram.Next(1, strs.Length)];
                    string fileName = "C:\\Users\\xiaoke\\Pictures\\ダウンロード　ピクチャ\\" + URL.Substring(URL.LastIndexOf("/"));

                    try
                    {
                        if (!File.Exists(fileName))
                        {
                            File.Create(fileName).Close();
                            MyWebClient.DownloadFile(URL, fileName);
                            SystemParametersInfo(20, 0, fileName, 0x1);
                        }
                    }
                    catch (SystemException s)
                    {
                        continue;
                    }
                    break;

                }

            }

            catch (WebException webEx)
            {

                Console.WriteLine(webEx.Message.ToString());

            }

        }

    }
}
