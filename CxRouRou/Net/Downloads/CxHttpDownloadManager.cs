using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// 下载管理器
    /// </summary>
    public class CxHttpDownloadManager : IDisposable
    {
        /// <summary>
        /// 进度处理任务
        /// </summary>
        private Task _progressHandlerTask;
        /// <summary>
        /// 取消操作
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;
        /// <summary>
        /// 进度处理间隔时间
        /// </summary>
        private readonly int _progressHandlerSpaceTime;
        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly object _syncLock;
        /// <summary>
        /// Http下载列表
        /// </summary>
        private readonly Dictionary<string, CxHttpDownload> _httpDownloadMap;
        /// <summary>
        /// 下载数据列表
        /// </summary>
        private readonly Dictionary<string, CxDownloadData> _downloadDataMap;
        /// <summary>
        /// 最大同时下载zhong任务数量
        /// </summary>
        private readonly int _maxDownloadingCount;
        /// <summary>
        /// 当前下载中任务数量
        /// </summary>
        private int _curDownloadingCount;
        /// <summary>
        /// 需要加载的任务数量
        /// </summary>
        private int _needDwonloadCount;
        /// <summary>
        /// 进度广播 已下载的字节数
        /// </summary>
        public Action<long> ProgressAction;
        /// <summary>
        /// 完成函数 是否发生错误 错误信息
        /// </summary>
        public Action<bool, string> CompleteAction;
        /// <summary>
        /// 错误字符串构造器
        /// </summary>
        private readonly StringBuilder _errorStringBuilder;
        /// <summary>
        /// 是否开始下载
        /// </summary>
        private bool _isStart = false;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool _isComplete = false;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="progressHandlerSpaceTime"></param>
        /// <param name="maxDownloadingCount"></param>
        /// <param name="taskCount">预计任务数量</param>
        public CxHttpDownloadManager(int progressHandlerSpaceTime = 500, int maxDownloadingCount = 5, int taskCount = 4)
        {
            if (progressHandlerSpaceTime <= 0)
            {
                throw new ArgumentException("param can not less than or equal to 0.", nameof(progressHandlerSpaceTime));
            }
            if (maxDownloadingCount <= 0)
            {
                throw new ArgumentException("param can not less than or equal to 0.", nameof(maxDownloadingCount));
            }
            _progressHandlerSpaceTime = progressHandlerSpaceTime;
            _maxDownloadingCount = maxDownloadingCount;
            _curDownloadingCount = 0;
            _needDwonloadCount = 0;
            _isStart = false;
            _isComplete = false;
            _errorStringBuilder = new StringBuilder();

            _syncLock = new object();
            _httpDownloadMap = new Dictionary<string, CxHttpDownload>(taskCount);
            _downloadDataMap = new Dictionary<string, CxDownloadData>(taskCount);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        public void AddTask(string url, string savePath)
        {
            lock (_syncLock)
            {
                if (_httpDownloadMap.ContainsKey(url))
                {
                    //无法添加重复url的任务
                    return;
                }
                var httpDownload = new CxHttpDownload(url, savePath)
                {
                    CompleteAction = HttpDownloadCompleteAction
                };
                var downloadData = httpDownload.GetDownloadData();
                _httpDownloadMap.Add(url, httpDownload);
                _downloadDataMap.Add(url, downloadData);
            }
        }
        /// <summary>
        /// Http下载完成回调
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="downloadData"></param>
        private void HttpDownloadCompleteAction(bool isError, CxDownloadData downloadData)
        {
            lock (_syncLock)
            {
                var url = downloadData.Url;
                var _downloadData = _downloadDataMap[url];
                if (_downloadData == null) return;
                _downloadData.UpdateValue(downloadData);
                _curDownloadingCount--;
                _needDwonloadCount--;
                if (_downloadData.State == EDownloadState.Error)
                {
                    _errorStringBuilder.AppendLine(CxString.Format("Url:{0} Error:{1}", url, _downloadData.ErrorMsg));
                }
            }
            if (_needDwonloadCount == 0)
            {
                CompleteAction?.Invoke(_errorStringBuilder.Length != 0, _errorStringBuilder.ToString());
                _isStart = false;
                _isComplete = true;
            }
            else
            {
                CheckStart();
            }
        }
        /// <summary>
        /// 检测任务开始
        /// </summary>
        private void CheckStart()
        {
            lock (_syncLock)
            {
                foreach (var kv in _httpDownloadMap)
                {
                    if (_curDownloadingCount < _maxDownloadingCount)
                    {
                        var httpDownload = kv.Value;
                        if (httpDownload.Start())
                        {
                            _curDownloadingCount++;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            lock (_syncLock)
            {
                if (_isStart || _isComplete) return;
                _errorStringBuilder.Clear();
                _needDwonloadCount = 0;
                //统计未完成的任务数量
                foreach (var kv in _httpDownloadMap)
                {
                    var httpDownload = kv.Value;
                    var downloadData = _downloadDataMap[kv.Key];
                    if (httpDownload != null && downloadData != null)
                    {
                        httpDownload.ClearError();
                        httpDownload.SetDownloadDataTo(downloadData);
                        if (downloadData.State != EDownloadState.Complete)
                        {
                            _needDwonloadCount++;
                        }
                    }
                }
                _cancellationTokenSource = new CancellationTokenSource();
                //取消标记
                var cancellationToken = _cancellationTokenSource.Token;
                _progressHandlerTask = Task.Factory.StartNew(ProgressHandlerThreadMethod, cancellationToken, cancellationToken);
                _isStart = true;
            }
            CheckStart();
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            lock (_syncLock)
            {
                if (!_isStart || _isComplete) return;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
                _progressHandlerTask = null;
                _needDwonloadCount = 0;
                foreach (var kv in _httpDownloadMap)
                {
                    if (_curDownloadingCount > 0)
                    {
                        var httpDownload = kv.Value;
                        if (httpDownload.Stop())
                        {
                            _curDownloadingCount--;
                        }
                    }
                }
                _isStart = false;
            }
        }
        /// <summary>
        /// 进度处理线程函数
        /// </summary>
        /// <param name="obj"></param>
        private void ProgressHandlerThreadMethod(object obj)
        {
            try
            {
                //转换取消标记
                var cancellationToken = (CancellationToken)obj;
                while (true)
                {
                    var downloadedSize = 0L;
                    lock (_syncLock)
                    {
                        foreach (var kv in _httpDownloadMap)
                        {
                            var httpDownload = kv.Value;
                            var downloadData = _downloadDataMap[kv.Key];
                            if (httpDownload != null && downloadData != null)
                            {
                                httpDownload.SetDownloadDataTo(downloadData);
                                downloadedSize += downloadData.DownloadedSize;
                            }
                        }
                    }
                    ProgressAction?.Invoke(downloadedSize);
                    downloadedSize = 0;
                    Thread.Sleep(_progressHandlerSpaceTime);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Stop();
            _errorStringBuilder.Clear();
            _httpDownloadMap.Clear();
            _downloadDataMap.Clear();
        }
    }
}
