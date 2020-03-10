using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CxSolution.CxRouRou.Collections;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// Http载
    /// </summary>
    public sealed class CxHttpDownload : CxDwonloadBase
    {
        /// <summary>
        /// 默认超时时间
        /// </summary>
        public static readonly TimeSpan DefTimeOut = new TimeSpan(10000000 * 20);
        /// <summary>
        /// 处理间隔时间(毫秒)
        /// </summary>
        public static int HandlerSpaceTime = 200;
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public const int BufferSize = 1024 * 80;
        /// <summary>
        /// 缓冲区池 
        /// </summary>
        private static readonly CxPool<byte[]> BufferPool;
        /// <summary>
        /// 进度函数
        /// </summary>
        public Action<CxDownloadData> ProgressAction;
        /// <summary>
        /// 完成函数
        /// </summary>
        public Action<bool, CxDownloadData> CompleteAction;
        /// <summary>
        /// HttpClient操作
        /// </summary>
        private static HttpClient HttpClientHandler;
        /// <summary>
        /// 下载任务
        /// </summary>
        private Task downloadTask;
        /// <summary>
        /// 取消操作
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static CxHttpDownload()
        {
            HttpClientHandler = new HttpClient
            {
                Timeout = DefTimeOut
            };
            BufferPool = new CxPool<byte[]>(1, () =>
            {
                return new byte[BufferSize];
            });
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePatch"></param>
        public CxHttpDownload(string url, string savePatch) : base(url, savePatch)
        {

        }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            Stop();
        }
        /// <summary>
        /// 开始
        /// </summary>
        public override int Start()
        {
            if (State == EDownloadState.Downloading)
            {
                //任务已在下载中
                return __ErrorCode.Success;
            }
            else if (State == EDownloadState.Complete)
            {
                //任务已经下载完成
                return __ErrorCode.Success;
            }
            //取消标记来源(相当于控制器)
            cancellationTokenSource = new CancellationTokenSource();
            //取消标记
            var cancellationToken = cancellationTokenSource.Token;
            //构造任务
            downloadTask = Task.Factory.StartNew(DownloadHandler, cancellationToken, cancellationToken);
            //设置状态为下载中
            State = EDownloadState.Downloading;
            //计算已下载进度
            Progress = 1f * DownloadedSize / FileSize;
            DownloadSpeed = 0;
            //调用一次进度函数
            ProgressAction?.Invoke(GetDownloadData());
            return __ErrorCode.Success;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public override int Stop()
        {
            if (State != EDownloadState.Downloading)
            {
                //任务未处于下载中,无法停止
                return __ErrorCode.Success;
            }
            cancellationTokenSource.Cancel();
            State = EDownloadState.Stop;
            //计算已下载进度
            Progress = 1f * DownloadedSize / FileSize;
            DownloadSpeed = 0;
            //调用一次进度函数
            ProgressAction?.Invoke(GetDownloadData());
            return __ErrorCode.Success;
        }
        /// <summary>
        /// 下载处理函数
        /// </summary>
        private void DownloadHandler(object obj)
        {
            try
            {
                //转换取消标记
                var cancellationToken = (CancellationToken)obj;
                //设置range测试是否支持断点续传
                HttpClientHandler.DefaultRequestHeaders.Range = new RangeHeaderValue(0, 0);
                //获取http请求头任务
                using (var httpResponseHeadersTask = HttpClientHandler.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead))
                {
                    //等待任务完成
                    while (!httpResponseHeadersTask.IsCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    //获取Http头信息
                    var httpHeadersMessage = httpResponseHeadersTask.Result;
                    //是否支持断点续传
                    var acceptRanges = false;
                    if (httpHeadersMessage.Content.Headers.ContentRange != null)
                    {
                        acceptRanges = true;
                    }
                    //ETAG标记
                    var eTag = httpHeadersMessage.Headers.ETag?.ToString() ?? "";
                    //最后一次修改时间
                    var lastModified = httpHeadersMessage.Content.Headers.LastModified?.ToString() ?? "";
                    //内容长度
                    var contentLength = 0L;
                    //如果支持断点续传,则内容长度从ContentRange字段获取,否则使用ContentLength
                    if (acceptRanges)
                        contentLength = (long)httpHeadersMessage.Content.Headers.ContentRange.Length;
                    else
                        contentLength = (long)httpHeadersMessage.Content.Headers.ContentLength;
                    //开始长度
                    var startLength = 0L;
                    //处理已下载信息
                    //已下载信息保存路径
                    var httpDownloadInfoFile = SavePath + ".mage";
                    //加载已下载信息
                    var httpDownloadInfo = CxHttpDownloadInfo.LoadFromFile(httpDownloadInfoFile);
                    //判断之前下载的文件在服务器是否已经过期,如果未过期,则进行续传
                    if (acceptRanges == true && eTag == httpDownloadInfo.ETAG && lastModified == httpDownloadInfo.LastModified && contentLength == httpDownloadInfo.ContentLength)
                    {
                        //此处需要做减一操作,此处代表的是文件bytes中的索引
                        startLength = httpDownloadInfo.DownloadedSize - 1;
                        if (startLength < 0)
                        {
                            startLength = 0;
                        }
                        //设置下载内容区间
                        HttpClientHandler.DefaultRequestHeaders.Range = new RangeHeaderValue(startLength, contentLength - 1);
                    }
                    else
                    {
                        //不支持断点续传和不需要断点续传的情况,清空range
                        HttpClientHandler.DefaultRequestHeaders.Range = null;
                    }
                    //保存已下载长度
                    DownloadedSize = httpDownloadInfo.DownloadedSize;
                    //保存内容长度到文件大小
                    FileSize = contentLength;
                    //计算已下载进度
                    Progress = 1f * DownloadedSize / FileSize;
                    DownloadSpeed = 0;
                    //调用一次进度函数
                    ProgressAction?.Invoke(GetDownloadData());
                    //更新已下载信息
                    httpDownloadInfo.ETAG = eTag;
                    httpDownloadInfo.LastModified = lastModified;
                    httpDownloadInfo.ContentLength = contentLength;
                    //获取Http内容流任务
                    using (var httpContentStreamTask = HttpClientHandler.GetStreamAsync(Url))
                    {
                        ////等待加载完成
                        //while (!httpContentStreamTask.IsCompleted)
                        //{
                        //    cancellationToken.ThrowIfCancellationRequested();
                        //}
                        //检测并创建保存路径的文件夹
                        var dir = Path.GetDirectoryName(SavePath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        //打开写入文件流
                        using (var localFileStream = File.Open(SavePath, FileMode.OpenOrCreate))
                        {
                            //验证文件长度是否为下载文件长度,如果不是,则修改文件长度
                            if (localFileStream.Length != FileSize)
                            {
                                localFileStream.SetLength(FileSize);
                                localFileStream.Flush();
                            }
                            //设置游标到开始位置
                            localFileStream.Seek(startLength, SeekOrigin.Begin);
                            //初始化缓冲区
                            var buffer = BufferPool.Pop();
                            var readLength = 0;
                            //计时器(计算下载速度)
                            var speedStopwatch = Stopwatch.StartNew();
                            //下载大小(计算下载速度)
                            var speedDownloadSize = 0L;
                            //获取Http内容流
                            using (var httpContentStream = httpContentStreamTask.Result)
                            {
                                //设置读取超时时间
                                httpContentStream.ReadTimeout = (int)DefTimeOut.TotalMilliseconds;
                                //循环读取Http内容流(已下载大小等于了文件大小或者无法读取出任何内容,则视为下载完成)
                                while ((DownloadedSize == FileSize || (readLength = httpContentStream.Read(buffer, 0, buffer.Length)) > 0))
                                {
                                    //写入本地文件
                                    localFileStream.Write(buffer, 0, readLength);
                                    //统计已下载内容
                                    DownloadedSize += readLength;
                                    speedDownloadSize += readLength;
                                    //计算已下载进度
                                    Progress = 1f * DownloadedSize / FileSize;
                                    //指定间隔进行一次以下操作
                                    if (speedStopwatch.ElapsedMilliseconds > HandlerSpaceTime)
                                    {
                                        //计算下载速度
                                        DownloadSpeed = speedDownloadSize * 1000 / speedStopwatch.ElapsedMilliseconds;
                                        speedDownloadSize = 0;
                                        speedStopwatch.Restart();
                                        //调用一次进度函数
                                        ProgressAction?.Invoke(GetDownloadData());
                                        //写入下载信息到本地
                                        httpDownloadInfo.DownloadedSize = DownloadedSize;
                                        CxHttpDownloadInfo.SaveToFile(httpDownloadInfoFile, httpDownloadInfo);
                                    }
                                    //判断是否取消了任务
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                                //计算已下载进度
                                Progress = 1f * DownloadedSize / FileSize;
                                DownloadSpeed = 0;
                                //删除已下载信息
                                File.Delete(httpDownloadInfoFile);
                                //下载完成
                                DownloadComplete();
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //该异常为取消任务所导致,不进行任何处理
                Progress = 1f * DownloadedSize / FileSize;
                DownloadSpeed = 0;
                //调用一次进度函数
                ProgressAction?.Invoke(GetDownloadData());
            }
            catch (AggregateException ae)
            {
                var sb = new StringBuilder();
                foreach (var innerException in ae.InnerExceptions)
                {
                    sb.AppendLine(innerException.Message);
                }
                DownloadError(sb.ToString());
            }
            catch (Exception e)
            {
                DownloadError(e.Message);
            }
        }
        /// <summary>
        /// 下载完成
        /// </summary>
        private void DownloadComplete()
        {
            //修改状态为完成
            State = EDownloadState.Complete;
            //调用一次完成函数
            CompleteAction?.Invoke(true, GetDownloadData());
        }
        /// <summary>
        /// 下载错误
        /// </summary>
        /// <param name="errorMsg"></param>
        private void DownloadError(string errorMsg)
        {
            ErrorMsg = errorMsg;
            State = EDownloadState.Error;
            Progress = 1f * DownloadedSize / FileSize;
            DownloadSpeed = 0;
            //调用一次完成函数
            CompleteAction?.Invoke(false, GetDownloadData());
        }
    }
}
