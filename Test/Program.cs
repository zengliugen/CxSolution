using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Reflection;
using Newtonsoft.Json;
using System.Threading.Tasks;

using static System.Console;
using System.Threading;
using CxSolution.CxRouRou.Net.Downloads;
using System.Net.Http.Headers;

namespace CxSolution.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://dl.softmgr.qq.com/original/Video/QQliveSetup_20_523_10.29.5563.0.exe";
            //url = "https://dl.softmgr.qq.com/original/Office/DeskGo_3_0_1409_127_lite.exe";
            //url = "https://tpc.googlesyndication.com/simgad/14512713954686523981/downsize_200k_v1?w=400&h=209";
            url = "https://docs.microsoft.com/_themes/docs.theme/master/zh-cn/_themes/scripts/e11f6b4f.index-docs.js";
            //string savePath = url.Substring(url.LastIndexOf('/') + 1);

            //CxHttpDownload cxHttpDownload = new CxHttpDownload(url, "temp/" + savePath);
            //cxHttpDownload.ProgressAction = (date) =>
            //{
            //    WriteLine("{0}:{1} Speed:{2}kb/s".FormatSelf(date.State, date.Progress, date.DownloadSpeed / 1024));
            //};
            //cxHttpDownload.CompleteAction = (ok, date) =>
            //{
            //    WriteLine("{0}:{1}".FormatSelf(date.State, date.Progress));
            //};
            //cxHttpDownload.Start();

            var HttpClientHandler = new HttpClient();

            //是否支持断点续传
            var acceptRanges = false;
            //设置range测试是否指出断点续传
            HttpClientHandler.DefaultRequestHeaders.Range = new RangeHeaderValue(0, 0);
            //获取检测是否支持断点续传请求头任务
            using (var checkAcceptRangesResponseHeadersTask = HttpClientHandler.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                //等待任务完成
                while (!checkAcceptRangesResponseHeadersTask.IsCompleted)
                {
                }
                //获取检测是否支持断点续传Http头信息
                var acceptRangesHeadersMessage = checkAcceptRangesResponseHeadersTask.Result;
                if (acceptRangesHeadersMessage.Content.Headers.ContentRange != null)
                {
                    acceptRanges = true;
                }
                WriteLine(acceptRangesHeadersMessage);
                WriteLine(acceptRangesHeadersMessage.Headers);
                WriteLine(acceptRangesHeadersMessage.Content.Headers);
                WriteLine(acceptRanges);
            }

            ReadKey();
        }
    }
}
