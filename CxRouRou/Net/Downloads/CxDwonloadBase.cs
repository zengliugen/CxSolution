using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// 任务
    /// </summary>
    public abstract class CxDwonloadBase : IDisposable
    {
        /// <summary>
        /// 下载数据
        /// </summary>
        protected readonly CxDownloadData _downloadData;
        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly object _syncLock = new object();
        /// <summary>
        /// 状态
        /// </summary>
        protected EDownloadState State
        {
            get
            {
                lock (_syncLock)
                {
                    return _downloadData.State;
                }
            }
            set
            {
                lock (_syncLock)
                {
                    _downloadData.State = value;
                }
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        protected string Url
        {
            get
            {
                lock (_syncLock)
                {
                    return _downloadData.Url;
                }
            }
        }
        /// <summary>
        /// 保存路径
        /// </summary>
        protected string SavePath
        {
            get
            {
                lock (_syncLock)
                {
                    return _downloadData.SavePath;
                }
            }
        }
        /// <summary>
        /// 已下载大小
        /// </summary>
        protected long DownloadedSize
        {
            get
            {
                lock (_syncLock)
                {
                    return _downloadData.DownloadedSize;
                }
            }
            set
            {
                lock (_syncLock)
                {
                    _downloadData.DownloadedSize = value;
                }
            }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        protected string ErrorMsg
        {
            set
            {
                lock (_syncLock)
                {
                    _downloadData.ErrorMsg = value;
                }
            }
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        protected long FileSize
        {
            get
            {
                lock (_syncLock)
                {
                    return _downloadData.FileSize;
                }
            }
            set
            {
                lock (_syncLock)
                {
                    _downloadData.FileSize = value;
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePatch"></param>
        public CxDwonloadBase(string url, string savePatch)
        {
            _downloadData = new CxDownloadData
            {
                State = EDownloadState.Ready,
                Url = url ?? throw new ArgumentNullException(nameof(url)),
                SavePath = savePatch ?? throw new ArgumentNullException(nameof(savePatch)),
                DownloadedSize = 0,
                FileSize = 0,
                ErrorMsg = null,
                DownloadSpeed = 0,
            };
        }
        /// <summary>
        /// 释放
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// 开始
        /// </summary>
        public abstract bool Start();
        /// <summary>
        /// 停止
        /// </summary>
        public abstract bool Stop();
        /// <summary>
        /// 清除错误 将任务状态设置为停止
        /// </summary>
        /// <returns></returns>
        public virtual bool ClearError()
        {
            if (State != EDownloadState.Error)
            {
                //未发生错误
                return false;
            }
            State = EDownloadState.Stop;
            ErrorMsg = "";
            return true;
        }
        /// <summary>
        /// 获取下载数据
        /// </summary>
        /// <returns></returns>
        public virtual CxDownloadData GetDownloadData()
        {
            lock (_syncLock)
            {
                return new CxDownloadData(_downloadData);
            }
        }
        /// <summary>
        /// 设置下载数据到参数
        /// </summary>
        /// <param name="downloadData"></param>
        public virtual void SetDownloadDataTo(CxDownloadData downloadData)
        {
            lock (_syncLock)
            {
                downloadData?.UpdateValue(_downloadData);
            }
        }
    }
}
