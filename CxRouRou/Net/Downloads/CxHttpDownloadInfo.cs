using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CxSolution.CxRouRou.Exceptions;
using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// Http下载信息(用于断点续传)
    /// </summary>
    public class CxHttpDownloadInfo
    {
        /// <summary>
        /// 标记
        /// </summary>
        public string ETAG;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastModified;
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long DownloadedSize;
        /// <summary>
        /// 内容大小
        /// </summary>
        public long ContentLength;
        /// <summary>
        /// 魔法标识
        /// </summary>
        public const uint MageFlag = 0x30333238;
        /// <summary>
        /// 从文件加载
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static CxHttpDownloadInfo LoadFromFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullOrEmptyException(nameof(file));
            }
            if (!File.Exists(file))
            {
                return new CxHttpDownloadInfo();
            }
            CxHttpDownloadInfo httpDownloadInfo = new CxHttpDownloadInfo();
            try
            {
                using (var fileStream = File.OpenRead(file))
                {
                    if (fileStream.Length > 4)
                    {
                        using (var binaryReader = new BinaryReader(fileStream))
                        {
                            var _MageFlag = binaryReader.ReadUInt32();
                            if (_MageFlag == MageFlag)
                            {
                                httpDownloadInfo.ETAG = binaryReader.ReadString();
                                httpDownloadInfo.LastModified = binaryReader.ReadString();
                                httpDownloadInfo.DownloadedSize = binaryReader.ReadInt64();
                                httpDownloadInfo.ContentLength = binaryReader.ReadInt64();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CxDebug.WriteLine("CxHttpDownloadInfo LoadFromFile Error:{0}", e.Message);
            }
            return httpDownloadInfo;
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="httpDownloadInfo"></param>
        public static void SaveToFile(string file, CxHttpDownloadInfo httpDownloadInfo)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullOrEmptyException(nameof(file));
            }
            if (httpDownloadInfo == null)
            {
                throw new ArgumentNullException(nameof(httpDownloadInfo));
            }
            //检测并创建保存路径的文件夹
            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            try
            {
                using (var fileStream = File.OpenWrite(file))
                {
                    using (var binaryWriter = new BinaryWriter(fileStream))
                    {
                        binaryWriter.Seek(0, SeekOrigin.Begin);
                        binaryWriter.Write(MageFlag);
                        binaryWriter.Write(httpDownloadInfo.ETAG);
                        binaryWriter.Write(httpDownloadInfo.LastModified);
                        binaryWriter.Write(httpDownloadInfo.DownloadedSize);
                        binaryWriter.Write(httpDownloadInfo.ContentLength);
                    }
                }
            }
            catch (Exception e)
            {
                CxDebug.WriteLine("CxHttpDownloadInfo SaveToFile Error:{0}", e.Message);
            }
        }
    }
}
