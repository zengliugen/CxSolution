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
    public abstract class CxDwonloadBase : IDownloadData, IDisposable
    {
        /// <summary>
        /// 状态
        /// </summary>
        public EDownloadState State { get; internal set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; internal set; }
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long DownloadedSize { get; internal set; }
        private float _progress;
        /// <summary>
        /// 下载进度
        /// </summary>
        public float Progress
        {
            get
            {
                return _progress;
            }
            internal set
            {
                if (float.IsNaN(value))
                {
                    value = 0;
                }
                _progress = value;
            }
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; internal set; }
        /// <summary>
        /// 下载速度
        /// </summary>
        public long DownloadSpeed { get; internal set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; internal set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePatch"></param>
        public CxDwonloadBase(string url, string savePatch)
        {
            State = EDownloadState.Ready;
            Url = url ?? throw new ArgumentNullException(nameof(url));
            SavePath = savePatch ?? throw new ArgumentNullException(nameof(savePatch));
        }
        /// <summary>
        /// 释放
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// 开始
        /// </summary>
        public abstract int Start();
        /// <summary>
        /// 停止
        /// </summary>
        public abstract int Stop();
        /// <summary>
        /// 获取下载数据
        /// </summary>
        /// <returns></returns>
        public virtual CxDownloadData GetDownloadData()
        {
            return new CxDownloadData(this);
        }
    }
}
