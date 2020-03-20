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
        public EDownloadState State { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long DownloadedSize { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 下载速度(以 字节/秒 计算)
        /// </summary>
        public long DownloadSpeed { get; set; }
        /// <summary>
        /// 上次修改时间(用于计算下载速度)
        /// </summary>
        private long LastModifyTicks { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        public CxDownloadData()
        {
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="downloadData"></param>
        public CxDownloadData(IDownloadData downloadData)
        {
            DownloadSpeed = 0;
            LastModifyTicks = DateTime.Now.Ticks;
            DownloadedSize = downloadData.DownloadedSize;
            UpdateValue(downloadData);
        }
        /// <summary>
        /// 更新值
        /// </summary>
        /// <param name="downloadData"></param>
        public void UpdateValue(IDownloadData downloadData)
        {
            if (State != downloadData.State)
                State = downloadData.State;
            if (Url != downloadData.Url)
                Url = downloadData.Url;
            if (SavePath != downloadData.SavePath)
                SavePath = downloadData.SavePath;
            if (FileSize != downloadData.FileSize)
                FileSize = downloadData.FileSize;
            if (ErrorMsg != downloadData.ErrorMsg)
                ErrorMsg = downloadData.ErrorMsg;
            UpdateDownloadedSize(downloadData.DownloadedSize);
        }
        /// <summary>
        /// 一秒对应的ticks
        /// </summary>
        private const long OneSecondTicks = 10000000;
        /// <summary>
        /// 更新已下载大小
        /// </summary>
        /// <param name="downloadedSize"></param>
        public void UpdateDownloadedSize(long downloadedSize)
        {
            var lastDownloadedSize = DownloadedSize;
            DownloadedSize = downloadedSize;
            var lastTicks = LastModifyTicks;
            var nowTicks = DateTime.Now.Ticks;
            LastModifyTicks = nowTicks;

            var changeSize = downloadedSize - lastDownloadedSize;
            var changeTicks = nowTicks - lastTicks;
            if (State != EDownloadState.Downloading)
            {
                DownloadSpeed = 0;
            }
            else if (changeSize > 0)
            {
                if (changeTicks > 0)
                {
                    //将ticks转换为秒后再进行计算
                    DownloadSpeed = (long)(changeSize / (1f * changeTicks / OneSecondTicks));
                }
            }
            else if (changeTicks > OneSecondTicks)
            {
                DownloadSpeed = 0;
            }
        }
    }
}
