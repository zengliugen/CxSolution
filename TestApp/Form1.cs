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

namespace TestApp
{
    public partial class Form1 : Form
    {
        private CxHttpDownload httpDownload;
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "https://dl.softmgr.qq.com/original/Video/QQliveSetup_20_523_10.29.5563.0.exe";
            var url = textBox1.Text;
            string savePath = "temp/" + url.Substring(url.LastIndexOf('/') + 1);
            httpDownload = new CxHttpDownload(url, savePath);
            Action<CxDownloadData> action = (date) =>
            {
                progressBar1.Value = (int)(date.Progress * 100);
                label2.Text = "{0}:{1} kb/s".FormatSelf(date.State, date.DownloadSpeed / 1024);
            };
            httpDownload.ProgressAction = (date) =>
            {
                Invoke(action, date);
            };
            Action<CxDownloadData> action2 = (date) =>
            {
                progressBar1.Value = (int)(date.Progress * 100);
                label2.Text = "{0}:{1}".FormatSelf(date.State, date.ErrorMsg);
            };
            httpDownload.CompleteAction = (ok, date) =>
            {
                Invoke(action2, date);
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            httpDownload.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            httpDownload.Stop();
        }
    }
}
