using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// 下载状态枚举
    /// </summary>
    public enum EDownloadState
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,
        /// <summary>
        /// 准备状态(即任务刚刚添加)
        /// </summary>
        Ready,
        /// <summary>
        /// 等待开始
        /// </summary>
        WaitingStart,
        /// <summary>
        /// 下载中
        /// </summary>
        Downloading,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 错误(下载发生了错误,重新开始任务即可)
        /// </summary>
        Error,
        /// <summary>
        /// 下载完成
        /// </summary>
        Complete,
    }
    /// <summary>
    /// 下载数据接口
    /// </summary>
    public interface IDownloadData
    {
        /// <summary>
        /// 状态
        /// </summary>
        EDownloadState State { get; }
        /// <summary>
        /// 地址
        /// </summary>
        string Url { get; }
        /// <summary>
        /// 保存路径
        /// </summary>
        string SavePath { get; }
        /// <summary>
        /// 已下载大小
        /// </summary>
        long DownloadedSize { get; }
        /// <summary>
        /// 文件大小
        /// </summary>
        long FileSize { get; }
        /// <summary>
        /// 进度值
        /// </summary>
        float Progress { get; }
        /// <summary>
        /// 下载速度
        /// </summary>
        long DownloadSpeed { get; }
        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrorMsg { get; }
    }
}
