using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CxSolution.CxRouRou.Net.Downloads;
using CxSolution.CxRouRou.Expands;
using System.Threading;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private CxHttpDownloadManager httpDownloadManager;
        public Form1()
        {
            InitializeComponent();

            label3.Text = "";
            label2.Text = "";

            httpDownloadManager = new CxHttpDownloadManager(maxDownloadingCount: 2);

            Action<long> progressAction = (size) =>
            {
                label2.Text = string.Format("downloaded size:{0}Mb{1}Kb{2}byte", size / 1024 / 1024, (size / 1024) % 1024, size % 1024);
            };
            httpDownloadManager.ProgressAction = (size) =>
            {
                Invoke(progressAction, size);
            };

            Action<bool, string> completeAction = (isError, errorMsg) =>
            {
                if (isError)
                {
                    label3.Text = "Load Error:\n" + errorMsg;
                }
                else
                {
                    label3.Text = "Load Ok";
                }
            };
            httpDownloadManager.CompleteAction = (isError, errorMsg) =>
            {
                Invoke(completeAction, isError, errorMsg);
            };

            var url = "https://tpc.googlesyndication.com/simgad/14512713954686523981/downsize_200k_v1?w=400&h=209";
            AddTask(url);

            url = "https://docs.microsoft.com/_themes/docs.theme/master/zh-cn/_themes/scripts/e11f6b4f.index-docs.js";
            AddTask(url);
        }

        void AddTask(string url)
        {
            string savePath = "temp/" + url.Substring(url.LastIndexOf('/') + 1);
            httpDownloadManager.AddTask(url, savePath);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            httpDownloadManager.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            httpDownloadManager.Stop();
        }
    }
}
