using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// 下载数据
    /// </summary>
    public class CxDownloadData : IDownloadData
    {
        /// <summary>
        /// 状态
        /// </summary>
        public EDownloadState State { get; private set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; private set; }
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long DownloadedSize { get; private set; }
        /// <summary>
        /// 进度
        /// </summary>
        public float Progress { get; private set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; private set; }
        /// <summary>
        /// 下载速度
        /// </summary>
        public long DownloadSpeed { get; private set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="downloadData"></param>
        public CxDownloadData(IDownloadData downloadData)
        {
            State = downloadData.State;
            Url = downloadData.Url;
            SavePath = downloadData.SavePath;
            DownloadedSize = downloadData.DownloadedSize;
            FileSize = downloadData.FileSize;
            Progress = downloadData.Progress;
            DownloadSpeed = downloadData.DownloadSpeed;
            ErrorMsg = downloadData.ErrorMsg;
        }
    }
}
